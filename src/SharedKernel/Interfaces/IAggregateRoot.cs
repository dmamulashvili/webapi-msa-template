using System;

namespace SharedKernel.Interfaces;

public interface IAggregateRoot
{
    Guid CorrelationId { get; }

    string ModifiedBy { get; }
    DateTimeOffset DateModified { get; }
    
    string CreatedBy { get; }
    DateTimeOffset DateCreated { get; }

    void SetCorrelationId(Guid correlationId);
    
    void SetModifiedBy(string modifiedBy);
    void SetDateModified(DateTimeOffset dateModified);
    
    void SetCreatedBy(string createdBy);
    void SetDateCreated(DateTimeOffset dateCreated);
}