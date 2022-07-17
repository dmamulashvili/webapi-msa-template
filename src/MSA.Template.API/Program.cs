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
using MSA.Template.API.Configuration;
using MSA.Template.API.Middlewares;
using MSA.Template.API.Services;
using MSA.Template.Core;
using MSA.Template.Infrastructure;
using MSA.Template.IntegrationEventHandlers;
using MSA.Template.IntegrationEventHandlers.Filters;
using MSA.Template.IntegrationEvents.Services;
using MSA.Template.SharedKernel.IntegrationEvents;
using MSA.Template.SharedKernel.Interfaces;
using SharedKernel.Audit.Interfaces;
using System.IO.Compression;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddControllers();
builder.Services.AddRouting(options => { options.LowercaseUrls = true; });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            // ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Please enter JWT with Bearer into field",
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
    });
});

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new DefaultCoreModule());
    containerBuilder.RegisterModule(new DefaultInfrastructureModule());
});

builder.Services.AddDbContext<MasterDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString(nameof(MasterDbContext)));
});

builder.Services.AddDbContext<SlaveDbContext>(optionsBuilder =>
{
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
builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });

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
    app.UseSwaggerUI();
}

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
    public static IServiceCollection AddMasstransitUsingAmazonSqs(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<MasterDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(20);

                o.UsePostgres();
                o.UseBusOutbox();
                o.DisableInboxCleanupService();
            });

            configurator.AddConsumers(typeof(OrderPaymentSucceededIntegrationEventHandler).Assembly);

            configurator.UsingAmazonSqs((context, cfg) =>
            {
                var amazonSqsConfig = configuration.GetSection(nameof(AmazonSqsConfiguration))
                    .Get<AmazonSqsConfiguration>();

                cfg.Host(amazonSqsConfig.RegionEndpointSystemName,
                    h =>
                    {
                        h.AccessKey(amazonSqsConfig.AccessKey);
                        h.SecretKey(amazonSqsConfig.SecretKey);

                        h.Scope(hostEnvironment.EnvironmentName, true);
                    });

                Guard.Against.NullOrWhiteSpace(amazonSqsConfig.QueueName, nameof(amazonSqsConfig.QueueName));

                cfg.ReceiveEndpoint($"{hostEnvironment.EnvironmentName}_{amazonSqsConfig.QueueName}",
                    e =>
                    {
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(5, TimeSpan.FromMinutes(1));
                            r.Ignore<ArgumentNullException>();
                        });

                        e.ConfigureConsumers(context);

                        e.UseConsumeFilter(typeof(IdentityConsumeContextFilter<>), context);

                        e.QueueAttributes.Add(QueueAttributeName.VisibilityTimeout,
                            TimeSpan.FromMinutes(20).TotalSeconds);
                        e.QueueAttributes.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, 20);
                        e.QueueAttributes.Add(QueueAttributeName.MessageRetentionPeriod,
                            TimeSpan.FromDays(10).TotalSeconds);
                        e.WaitTimeSeconds = 20;
                    });
            });
        });

        return services;
    }
}