using Ardalis.GuardClauses;
using MassTransit;
using SharedKernel.IntegrationEvents;
using SharedKernel.Interfaces;

namespace MSA.Template.BackgroundTasks.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IServiceProvider _serviceProvider;

    // private readonly IBus _eventBus;
    private readonly IIdentityService _identityService;
    private readonly ILogger<IntegrationEventService> _logger;
    private readonly List<IIntegrationEvent> _events = new List<IIntegrationEvent>();

    public IntegrationEventService(
        IServiceProvider serviceProvider,
        // IBus eventBus,
        IIdentityService identityService,
        ILogger<IntegrationEventService> logger
    )
    {
        _serviceProvider = Guard.Against.Null(serviceProvider);
        // _eventBus = eventBus;
        _identityService = Guard.Against.Null(identityService);
        _logger = Guard.Against.Null(logger);
    }

    public Task AddEventAsync(IIntegrationEvent @event)
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public async Task PublishEventsAsync(Guid correlationId, CancellationToken cancellationToken)
    {
        var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();
        foreach (var @event in _events)
        {
            await publishEndpoint.Publish(
                @event,
                @event.GetType(),
                c =>
                {
                    c.CorrelationId = correlationId;
                    c.InitiatorId = _identityService.GetUserIdentity();
                },
                cancellationToken
            );
        }

        _events.Clear();
    }
}
