using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MSA.Template.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MasterDbContext>(optionsBuilder =>
{
    var connStringMasterDbContext = Environment.GetEnvironmentVariable(
        "ConnectionString_MasterDbContext"
    );
    Guard.Against.NullOrWhiteSpace(connStringMasterDbContext, nameof(connStringMasterDbContext));

    optionsBuilder.UseNpgsql(connStringMasterDbContext);
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

    configurator.UsingAmazonSqs(
        (context, cfg) =>
        {
            var projectShortName = builder.Configuration["Project:ShortName"];

            Guard.Against.NullOrWhiteSpace(projectShortName, nameof(projectShortName));

            var amazonSqsAccessKey = Environment.GetEnvironmentVariable("AmazonSQS_AccessKey");
            var amazonSqsSecretKey = Environment.GetEnvironmentVariable("AmazonSQS_SecretKey");
            var amazonSqsRegionEndpointSystemName = Environment.GetEnvironmentVariable(
                "AmazonSQS_RegionEndpointSystemName"
            );

            cfg.Host(
                amazonSqsRegionEndpointSystemName,
                h =>
                {
                    h.AccessKey(amazonSqsAccessKey);
                    h.SecretKey(amazonSqsSecretKey);

                    h.Scope($"{builder.Environment.EnvironmentName}_{projectShortName}", true);
                }
            );
        }
    );
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
