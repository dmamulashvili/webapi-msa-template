using System.Threading.Tasks;

namespace MSA.Template.SharedKernel.IntegrationEvents;

public interface IIntegrationEventService
{
    Task AddAsync(IIntegrationEvent @event);
    Task PublishAllAsync();
}