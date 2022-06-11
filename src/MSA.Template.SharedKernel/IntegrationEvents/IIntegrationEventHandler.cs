using MassTransit;

namespace MSA.Template.SharedKernel.IntegrationEvents;

public interface IIntegrationEventHandler<T> : IConsumer<T>
    where T : class, IIntegrationEvent
{
}