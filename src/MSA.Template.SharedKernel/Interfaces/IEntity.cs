using System.Collections.Generic;

namespace MSA.Template.SharedKernel.Interfaces;

public interface IEntity
{
    IReadOnlyCollection<BaseDomainEvent>? DomainEvents { get; }
    
    void RemoveDomainEvent(BaseDomainEvent eventItem);
    void ClearDomainEvents();
}