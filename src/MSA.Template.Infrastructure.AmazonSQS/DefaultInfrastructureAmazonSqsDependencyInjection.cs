using Amazon.SQS;
using Ardalis.GuardClauses;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSA.Template.Infrastructure.AmazonSQS.Configuration;
using MSA.Template.Infrastructure.AmazonSQS.Formatters;
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

            configurator.UsingAmazonSqs((context, amazonSqsBusFactoryConfigurator) =>
            {
                var amazonSqsConfiguration = configuration.GetSection(nameof(AmazonSqsConfiguration))
                    .Get<AmazonSqsConfiguration>();

                amazonSqsBusFactoryConfigurator.Host(amazonSqsConfiguration.RegionEndpointSystemName,
                    hostConfigurator =>
                    {
                        hostConfigurator.AccessKey(amazonSqsConfiguration.AccessKey);
                        hostConfigurator.SecretKey(amazonSqsConfiguration.SecretKey);
                    });

                amazonSqsBusFactoryConfigurator.MessageTopology.SetEntityNameFormatter(
                    new AmazonSqsEnvironmentNameFormatter(
                        amazonSqsBusFactoryConfigurator.MessageTopology.EntityNameFormatter,
                        hostEnvironment.EnvironmentName));

                var queueName = amazonSqsConfiguration.QueueName;

                Guard.Against.NullOrWhiteSpace(queueName, nameof(queueName));

                amazonSqsBusFactoryConfigurator.OverrideDefaultBusEndpointQueueName(
                    $"{queueName}_{hostEnvironment.EnvironmentName}_TEMP");

                amazonSqsBusFactoryConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(5, TimeSpan.FromMinutes(1));
                    retryConfigurator.Ignore<ArgumentNullException>();
                });

                amazonSqsBusFactoryConfigurator.ReceiveEndpoint($"{queueName}_{hostEnvironment.EnvironmentName}",
                    endpointConfigurator =>
                    {
                        endpointConfigurator.QueueAttributes.Add(QueueAttributeName.VisibilityTimeout,
                            TimeSpan.FromMinutes(20).TotalSeconds);
                        endpointConfigurator.QueueAttributes.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, 20);
                        endpointConfigurator.QueueAttributes.Add(QueueAttributeName.MessageRetentionPeriod,
                            TimeSpan.FromDays(10).TotalSeconds);
                        endpointConfigurator.WaitTimeSeconds = 20;

                        endpointConfigurator.UseConsumeFilter(typeof(IdentityConsumeContextFilter<>), context);
                        endpointConfigurator.ConfigureConsumers(context);
                    });

                amazonSqsBusFactoryConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}