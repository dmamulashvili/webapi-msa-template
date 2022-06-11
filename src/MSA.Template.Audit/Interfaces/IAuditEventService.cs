namespace MSA.Template.Audit.Interfaces;

public interface IAuditEventService
{
    Task AddEventAsync(BaseAuditEvent @event);
    Task PublishEventsAsync(CancellationToken cancellationToken);
}