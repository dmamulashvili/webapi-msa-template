using MSA.Template.SharedKernel.Interfaces;
using System;

namespace MSA.Template.SharedKernel;

public abstract class BaseAggregateRoot<TId> : BaseEntity<TId>, IAggregateRoot
{
    public Guid CorrelationId { get; private set; }

    public string ModifiedBy { get; private set; } = null!;
    public DateTimeOffset DateModified { get; private set; }

    public string CreatedBy { get; private set; } = null!;
    public DateTimeOffset DateCreated { get; private set; }

    void IAggregateRoot.SetCorrelationId(Guid correlationId) => CorrelationId = correlationId;

    void IAggregateRoot.SetModifiedBy(string modifiedBy) => ModifiedBy = modifiedBy;
    void IAggregateRoot.SetDateModified(DateTimeOffset dateModified) => DateModified = dateModified;

    void IAggregateRoot.SetCreatedBy(string createdBy) => CreatedBy = createdBy;
    void IAggregateRoot.SetDateCreated(DateTimeOffset dateCreated) => DateCreated = dateCreated;
}