using MassTransit;

namespace SharedKernel.IntegrationEvents;

public interface IIntegrationEventHandler<T> : IConsumer<T>
    where T : class, IIntegrationEvent
{
}