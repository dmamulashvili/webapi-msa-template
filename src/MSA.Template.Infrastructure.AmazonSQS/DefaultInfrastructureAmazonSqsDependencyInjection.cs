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
                        
                        hostConfigurator.Scope(hostEnvironment.EnvironmentName, true);
                    });

                amazonSqsBusFactoryConfigurator.MessageTopology.SetEntityNameFormatter(
                    new AmazonSqsEnvironmentNameFormatter(
                        amazonSqsBusFactoryConfigurator.MessageTopology.EntityNameFormatter,
                        hostEnvironment.EnvironmentName));

                Guard.Against.NullOrWhiteSpace(amazonSqsConfiguration.QueueName, nameof(amazonSqsConfiguration.QueueName));

                amazonSqsBusFactoryConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(5, TimeSpan.FromMinutes(1));
                    retryConfigurator.Ignore<ArgumentNullException>();
                });

                amazonSqsBusFactoryConfigurator.ReceiveEndpoint($"{hostEnvironment.EnvironmentName}_{amazonSqsConfiguration.QueueName}",
                    endpointConfigurator =>
                    {
                        endpointConfigurator.UseMessageRetry(r =>
                        {
                            r.Interval(5, TimeSpan.FromMinutes(1));
                            r.Ignore<ArgumentNullException>();
                        });
                        
                        endpointConfigurator.ConfigureConsumers(context);

                        endpointConfigurator.UseConsumeFilter(typeof(IdentityConsumeContextFilter<>), context);
                        
                        endpointConfigurator.QueueAttributes.Add(QueueAttributeName.VisibilityTimeout,
                            TimeSpan.FromMinutes(20).TotalSeconds);
                        endpointConfigurator.QueueAttributes.Add(QueueAttributeName.ReceiveMessageWaitTimeSeconds, 20);
                        endpointConfigurator.QueueAttributes.Add(QueueAttributeName.MessageRetentionPeriod,
                            TimeSpan.FromDays(10).TotalSeconds);
                        endpointConfigurator.WaitTimeSeconds = 20;
                    });
            });
        });

        return services;
    }
}