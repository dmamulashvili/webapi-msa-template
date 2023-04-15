using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SharedKernel.Interfaces;

namespace SharedKernel;

public abstract class BaseEntity<TId> : IEntity
{
    public TId Id { get; set; } = default!;

    private List<BaseDomainEvent>? _domainEvents;

    [NotMapped]
    public IReadOnlyCollection<BaseDomainEvent>? DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(BaseDomainEvent eventItem)
    {
        _domainEvents ??= new List<BaseDomainEvent>();
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(BaseDomainEvent eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}
