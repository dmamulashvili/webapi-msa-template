using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.SharedKernel.IntegrationEvents;

public interface IIntegrationEventService
{
    Task AddEventAsync(IIntegrationEvent @event);
    Task PublishEventsAsync(CancellationToken cancellationToken);
}