using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSA.Template.Audit.Interfaces;

namespace MSA.Template.Audit.Services;

public class AuditEventService : IAuditEventService
{
    private readonly IBus _eventBus;
    private readonly ILogger<AuditEventService> _logger;
    private readonly List<IAuditEvent> _events = new();
    // private readonly IServiceProvider _serviceProvider;

    public AuditEventService(IBus eventBus, ILogger<AuditEventService> logger 
        // ,IServiceProvider serviceProvider
        )
    {
        _eventBus = eventBus;
        _logger = logger;
        // _serviceProvider = serviceProvider;
    }

    public Task AddEventAsync(BaseAuditEvent @event)
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public async Task PublishEventsAsync(CancellationToken cancellationToken)
    {
        // TODO: To enable OutBox uncomment below functionality and replace _eventBus with publishEndpoint. Also view TransactionBehaviour.cs
        // var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();
        foreach (var @event in _events)
        {
            await _eventBus.Publish(@event, @event.GetType(), c =>
            {
                c.CorrelationId = @event.CorrelationId;
                c.InitiatorId = @event.InitiatorId;
            }, cancellationToken);
        }
    }
}