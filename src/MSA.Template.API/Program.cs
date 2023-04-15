using System.IO.Compression;
using System.Text;
using Amazon.SQS;
using Ardalis.GuardClauses;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MSA.Template.API.Filters;
using MSA.Template.API.Middlewares;
using MSA.Template.API.Services;
using MSA.Template.Core;
using MSA.Template.Infrastructure;
using MSA.Template.IntegrationEventHandlers;
using MSA.Template.IntegrationEventHandlers.Filters;
using SharedKernel.Audit.Interfaces;
using SharedKernel.IntegrationEvents;
using SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddControllers(options =>
{
    options.Filters.Add<DomainExceptionFilterAttribute>();
});
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "CorsPolicy",
        cfg =>
            cfg
            // .WithOrigins(builder.Configuration.GetSection("Cors:AllowOrigins").Get<string[]>())
            .SetIsOriginAllowed(_ => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromDays(1))
    );
});
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        var jwtValidAudience = Environment.GetEnvironmentVariable("JWT_ValidAudience");
        var jwtValidIssuer = Environment.GetEnvironmentVariable("JWT_ValidIssuer");
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_Secret");

        Guard.Against.NullOrWhiteSpace(jwtValidAudience, nameof(jwtValidAudience));
        Guard.Against.NullOrWhiteSpace(jwtValidIssuer, nameof(jwtValidIssuer));
        Guard.Against.NullOrWhiteSpace(jwtSecret, nameof(jwtSecret));

        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            // ValidAudience = jwtValidIssuer,
            ValidateIssuer = true,
            ValidIssuer = jwtValidIssuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Please enter JWT with Bearer into field",
            Type = SecuritySchemeType.ApiKey
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    },
                    Name = JwtBearerDefaults.AuthenticationScheme,
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        }
    );
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultCoreModule());
    containerBuilder.RegisterModule(new DefaultInfrastructureModule());
});

builder.Services.AddDbContext<MasterDbContext>(optionsBuilder =>
{
    var connStringMasterDbContext = Environment.GetEnvironmentVariable(
        "ConnectionString_MasterDbContext"
    );
    Guard.Against.NullOrWhiteSpace(connStringMasterDbContext, nameof(connStringMasterDbContext));

    optionsBuilder
        .UseLazyLoadingProxies()
        .UseNpgsql(
            connStringMasterDbContext,
            options =>
            {
                // options.EnableRetryOnFailure();
            }
        );
});

builder.Services.AddDbContext<SlaveDbContext>(optionsBuilder =>
{
    var connStringSlaveDbContext = Environment.GetEnvironmentVariable(
        "ConnectionString_SlaveDbContext"
    );
    Guard.Against.NullOrWhiteSpace(connStringSlaveDbContext, nameof(connStringSlaveDbContext));

    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString(nameof(SlaveDbContext)));
});

builder.Services.AddMasstransitUsingAmazonSqs(builder.Configuration, builder.Environment);
builder.Services.AddScoped<IIntegrationEventService, IntegrationEventService>();
builder.Services.AddScoped<IAuditEventService, AuditEventService>();

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.EnableForHttps = true;
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork>(p => p.GetRequiredService<MasterDbContext>());
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<IIdentityServiceProvider>(p => p.GetRequiredService<IdentityService>());
builder.Services.AddScoped<IIdentityService>(p => p.GetRequiredService<IdentityService>());

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(cfg =>
    {
        cfg.DefaultModelsExpandDepth(-1); // Disable swagger schemas at bottom
    });
}

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IdentityMiddleware>();

app.MapControllers();

app.UseResponseCompression();

app.MapHealthChecks("/");

app.Services.GetRequiredService<MasterDbContext>().Database.Migrate();

app.Run();

static class CustomExtensionsMethods
{
    public static IServiceCollection AddMasstransitUsingAmazonSqs(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment
    )
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<MasterDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(20);

                o.UsePostgres();
                o.UseBusOutbox(b => b.DisableDeliveryService());
                o.DisableInboxCleanupService();
            });

            configurator.AddConsumers(
                typeof(OrderPaymentSucceededIntegrationEventHandler).Assembly
            );

            configurator.UsingAmazonSqs(
                (context, cfg) =>
                {
                    var projectShortName = configuration["Project:ShortName"];

                    Guard.Against.NullOrWhiteSpace(projectShortName, nameof(projectShortName));

                    var amazonSqsAccessKey = Environment.GetEnvironmentVariable(
                        "AmazonSQS_AccessKey"
                    );
                    var amazonSqsSecretKey = Environment.GetEnvironmentVariable(
                        "AmazonSQS_SecretKey"
                    );
                    var amazonSqsRegionEndpointSystemName = Environment.GetEnvironmentVariable(
                        "AmazonSQS_RegionEndpointSystemName"
                    );

                    cfg.Host(
                        amazonSqsRegionEndpointSystemName,
                        h =>
                        {
                            h.AccessKey(amazonSqsAccessKey);
                            h.SecretKey(amazonSqsSecretKey);

                            h.Scope($"{hostEnvironment.EnvironmentName}_{projectShortName}", true);
                        }
                    );

                    cfg.ReceiveEndpoint(
                        $"{hostEnvironment.EnvironmentName}_{projectShortName}",
                        e =>
                        {
                            e.UseMessageRetry(r =>
                            {
                                r.Interval(5, TimeSpan.FromMinutes(1));
                            });

                            e.ConfigureConsumers(context);

                            e.UseConsumeFilter(typeof(IdentityConsumeContextFilter<>), context);

                            e.QueueAttributes.Add(
                                QueueAttributeName.VisibilityTimeout,
                                TimeSpan.FromMinutes(20).TotalSeconds
                            );
                            e.QueueAttributes.Add(
                                QueueAttributeName.ReceiveMessageWaitTimeSeconds,
                                20
                            );
                            e.QueueAttributes.Add(
                                QueueAttributeName.MessageRetentionPeriod,
                                TimeSpan.FromDays(10).TotalSeconds
                            );
                            e.WaitTimeSeconds = 20;
                        }
                    );
                }
            );
        });

        return services;
    }
}
