using Amazon.SQS;
using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSA.Template.Infrastructure.AmazonSQS.Configuration;
using MSA.Template.IntegrationEventHandlers;
using MSA.Template.IntegrationEventHandlers.Filters;

namespace MSA.Template.Infrastructure.AmazonSQS;

public static class DefaultInfrastructureAmazonSqsDependencyInjection
{
    public static IServiceCollection AddMasstransitUsingAmazonSqs(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        services.AddMassTransit(configurator =>
        {
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