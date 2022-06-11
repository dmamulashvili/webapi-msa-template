namespace MSA.Template.Audit.Interfaces;

public interface IAuditEventService
{
    Task AddAsync(BaseAuditEvent @event);
    Task PublishAllAsync();
}