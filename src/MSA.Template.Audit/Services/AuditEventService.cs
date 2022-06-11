using MassTransit;
using Microsoft.Extensions.Logging;
using MSA.Template.Audit.Interfaces;

namespace MSA.Template.Audit.Services;

public class AuditEventService : IAuditEventService
{
    private readonly IBus _bus;
    private readonly ILogger<AuditEventService> _logger;
    private readonly List<IAuditEvent> _events = new();

    public AuditEventService(IBus bus, ILogger<AuditEventService> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public Task AddEventAsync(BaseAuditEvent @event)
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public async Task PublishEventsAsync(CancellationToken cancellationToken)
    {
        foreach (var @event in _events)
        {
            await _bus.Publish(@event, @event.GetType(), c =>
            {
                c.CorrelationId = @event.CorrelationId;
                c.InitiatorId = @event.InitiatorId;
            }, cancellationToken);
        }
    }
}