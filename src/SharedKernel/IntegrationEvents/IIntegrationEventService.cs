using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.IntegrationEvents;

public interface IIntegrationEventService
{
    Task AddEventAsync(BaseIntegrationEvent @event);
    Task PublishEventsAsync(CancellationToken cancellationToken);
}