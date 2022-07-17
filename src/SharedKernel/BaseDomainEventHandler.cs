using SharedKernel.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel;

public abstract class BaseDomainEventHandler<T> : IDomainEventHandler<T> where T : BaseDomainEvent
{
    public abstract Task Handle(T @event, CancellationToken cancellationToken);
}