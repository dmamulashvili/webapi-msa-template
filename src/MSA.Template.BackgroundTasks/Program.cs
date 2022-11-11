using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MSA.Template.BackgroundTasks.Configuration;
using MSA.Template.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MasterDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString(nameof(MasterDbContext)));
});

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddEntityFrameworkOutbox<MasterDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(20);

        o.UsePostgres();
        o.UseBusOutbox();
        o.DisableInboxCleanupService();
    });
            
    configurator.UsingAmazonSqs((context, cfg) =>
    {
        var amazonSqsConfig = builder.Configuration.GetSection(nameof(AmazonSqsConfiguration))
            .Get<AmazonSqsConfiguration>();

        Guard.Against.NullOrWhiteSpace(amazonSqsConfig.Scope, nameof(amazonSqsConfig.Scope));

        cfg.Host(amazonSqsConfig.RegionEndpointSystemName,
            h =>
            {
                h.AccessKey(amazonSqsConfig.AccessKey);
                h.SecretKey(amazonSqsConfig.SecretKey);

                h.Scope($"{builder.Environment.EnvironmentName}_{amazonSqsConfig.Scope}", true);
            });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/");

app.Run();