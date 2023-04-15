using System.Collections.Generic;

namespace SharedKernel.Interfaces;

public interface IEntity
{
    IReadOnlyCollection<BaseDomainEvent>? DomainEvents { get; }

    void RemoveDomainEvent(BaseDomainEvent eventItem);
    void ClearDomainEvents();
}
