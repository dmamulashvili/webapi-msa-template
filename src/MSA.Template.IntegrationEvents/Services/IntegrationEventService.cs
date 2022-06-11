using MassTransit;
using Microsoft.Extensions.Logging;
using MSA.Template.SharedKernel;
using MSA.Template.SharedKernel.IntegrationEvents;
using MSA.Template.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.IntegrationEvents.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IBus _eventBus;
    private readonly IIdentityService _identityService;
    private readonly ILogger<IntegrationEventService> _logger;
    private readonly List<IIntegrationEvent> _events = new List<IIntegrationEvent>();

    public IntegrationEventService(IBus eventBus, IIdentityService identityService,
        ILogger<IntegrationEventService> logger)
    {
        _eventBus = eventBus;
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
        foreach (var @event in _events)
        {
            await _eventBus.Publish(@event, @event.GetType(), c =>
            {
                c.CorrelationId = @event.CorrelationId;
                c.InitiatorId = @event.InitiatorId;
            }, cancellationToken);
        }

        _events.Clear();
    }
}