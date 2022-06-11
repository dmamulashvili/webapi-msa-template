using MediatR;

namespace MSA.Template.SharedKernel.Interfaces;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : BaseDomainEvent
{
}