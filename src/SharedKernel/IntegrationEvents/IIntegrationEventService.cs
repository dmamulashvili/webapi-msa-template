using System;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.IntegrationEvents;

public interface IIntegrationEventService
{
    Task AddEventAsync(IIntegrationEvent @event);
    Task PublishEventsAsync(Guid correlationId, CancellationToken cancellationToken);
}
