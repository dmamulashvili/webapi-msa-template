namespace SharedKernel.Audit.Interfaces;

public interface IAuditEventService
{
    Task AddEventAsync(BaseAuditEvent @event);
    Task PublishEventsAsync(CancellationToken cancellationToken);
}