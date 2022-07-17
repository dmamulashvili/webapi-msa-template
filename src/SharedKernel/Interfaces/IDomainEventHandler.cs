using MediatR;

namespace SharedKernel.Interfaces;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : BaseDomainEvent
{
}