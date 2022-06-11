using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using MSA.Template.SharedKernel.IntegrationEvents;
using MSA.Template.SharedKernel.Interfaces;

namespace MSA.Template.IntegrationEvents.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly IBus _eventBus;
    private readonly IIdentityService _identityService;
    private readonly ILogger<IntegrationEventService> _logger;
    private readonly List<IIntegrationEvent> _events = new List<IIntegrationEvent>();

    public IntegrationEventService(IBus eventBus, IIdentityService identityService, ILogger<IntegrationEventService> logger)
    {
        _eventBus = eventBus;
        _identityService = identityService;
        _logger = logger;
    }

    public Task AddAsync(IIntegrationEvent @event)
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public async Task PublishAllAsync()
    {
        await Task.WhenAll(_events.Select(e => PublishWithRetry(e, 2)));
        _events.Clear();
    }

    private async Task PublishWithRetry(IIntegrationEvent @event, int retriesLeft)
    {
        try
        {
            await _eventBus.Publish(@event, @event.GetType(),c => c.InitiatorId = _identityService.GetUserIdentity());
        }
        catch
        {
            if (retriesLeft > 0)
            {
                await PublishWithRetry(@event, --retriesLeft);
            }
            else
            {
                throw;
            }
        }
    }
}