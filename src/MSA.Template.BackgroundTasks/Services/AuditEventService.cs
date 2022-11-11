using Ardalis.GuardClauses;
using MassTransit;
using SharedKernel.Audit;
using SharedKernel.Audit.Interfaces;

namespace MSA.Template.BackgroundTasks.Services;

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
        _eventBus = Guard.Against.Null(eventBus);
        _logger = Guard.Against.Null(logger);
        // _serviceProvider = Guard.Against.Null(serviceProvider);
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
        foreach (var @events in _events.GroupBy(e => new { e.CorrelationId, e.InitiatorId }))
        {
            await _eventBus.PublishBatch(@events, @events.First().GetType(), c =>
            {
                c.CorrelationId = @events.Key.CorrelationId;
                c.InitiatorId = @events.Key.InitiatorId;
            }, cancellationToken);
        }

        _events.Clear();
    }
}