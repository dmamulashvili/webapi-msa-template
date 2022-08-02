using MassTransit;
using SharedKernel;
using SharedKernel.IntegrationEvents;
using SharedKernel.Interfaces;

namespace MSA.Template.API.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IServiceProvider _serviceProvider;
    // private readonly IBus _eventBus;
    private readonly IIdentityService _identityService;
    private readonly ILogger<IntegrationEventService> _logger;
    private readonly List<IIntegrationEvent> _events = new List<IIntegrationEvent>();

    public IntegrationEventService(IServiceProvider serviceProvider, 
        // IBus eventBus, 
        IIdentityService identityService,
        ILogger<IntegrationEventService> logger)
    {
        _serviceProvider = serviceProvider;
        // _eventBus = eventBus;
        _identityService = identityService;
        _logger = logger;
    }

    public Task AddEventAsync(BaseIntegrationEvent @event)
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public async Task PublishEventsAsync(CancellationToken cancellationToken)
    {
        var publishEndpoint = _serviceProvider.GetRequiredService<IPublishEndpoint>();
        foreach (var @event in _events)
        {
            await publishEndpoint.Publish(@event, @event.GetType(), c =>
            {
                c.CorrelationId = @event.CorrelationId;
                c.InitiatorId = _identityService.GetUserIdentity();
            }, cancellationToken);
        }

        _events.Clear();
    }
}