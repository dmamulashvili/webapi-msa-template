using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using IntegrationEvents;
using MSA.Template.Core.OrderAggregate.Events;
using SharedKernel;
using SharedKernel.IntegrationEvents;

namespace MSA.Template.Core.OrderAggregate.EventHandlers;

public class OrderPlacedDomainEventHandler : BaseDomainEventHandler<OrderPlacedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public OrderPlacedDomainEventHandler(IIntegrationEventService integrationEventService)
    {
        _integrationEventService = Guard.Against.Null(integrationEventService);
    }

    // INFO: e.g. publish fot Notification API to notify customer about succeeded order placement.
    public override async Task Handle(
        OrderPlacedDomainEvent @event,
        CancellationToken cancellationToken
    )
    {
        var orderPlacedIntegrationEvent = new OrderPlacedIntegrationEvent(
            @event.Order.Id,
            @event.Order.OrderDate,
            @event.Order.OrderLines
                .Select(
                    ol =>
                        new OrderPlacedIntegrationEvent.OrderLineIntegrationEventDto(
                            ol.ItemId,
                            ol.ItemPrice,
                            ol.Quantity
                        )
                )
                .ToList()
        );

        await _integrationEventService.AddEventAsync(orderPlacedIntegrationEvent);
    }
}
