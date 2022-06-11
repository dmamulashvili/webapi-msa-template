using MSA.Template.Core.OrderAggregate.Events;
using MSA.Template.IntegrationEvents;
using MSA.Template.SharedKernel;
using MSA.Template.SharedKernel.IntegrationEvents;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.Core.OrderAggregate.EventHandlers;

public class OrderPlacedDomainEventHandler : BaseDomainEventHandler<OrderPlacedDomainEvent>
{
    private readonly IIntegrationEventService _integrationEventService;

    public OrderPlacedDomainEventHandler(
        IIntegrationEventService integrationEventService
    )
    {
        _integrationEventService =
            integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
    }

    // INFO: e.g. publish fot Notification API to notify customer about succeeded order placement.
    public override Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
    {
        var orderPlacedIntegrationEvent = new OrderPlacedIntegrationEvent(
            @event.Order.CorrelationId,
            @event.Order.Id,
            @event.Order.UserId.ToString(),
            @event.Order.OrderDate,
            @event.Order.OrderItems.Select(x =>
                new OrderPlacedIntegrationEvent.OrderLine(
                    x.ItemId,
                    x.ItemPrice,
                    x.Quantity)).ToList());
        
        return _integrationEventService.AddAsync(orderPlacedIntegrationEvent);
    }
}