using SharedKernel.Audit.Interfaces;

namespace SharedKernel.Audit;

public abstract class BaseAuditEvent : IAuditEvent
{
    public Guid CorrelationId { get; protected init; }
    public Guid InitiatorId { get; protected init; }
    public DateTimeOffset CreationDate { get; protected init; }
}