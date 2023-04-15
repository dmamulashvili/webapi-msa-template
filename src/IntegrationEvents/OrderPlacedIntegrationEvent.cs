using System;
using System.Collections.Generic;
using SharedKernel.IntegrationEvents;

namespace IntegrationEvents;

public class OrderPlacedIntegrationEvent : IIntegrationEvent
{
    public OrderPlacedIntegrationEvent(
        Guid orderId,
        DateTimeOffset orderDate,
        IReadOnlyCollection<OrderLineIntegrationEventDto> orderLines
    )
    {
        OrderId = orderId;
        OrderDate = orderDate;
        OrderLines = orderLines;
    }

    public Guid OrderId { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public IReadOnlyCollection<OrderLineIntegrationEventDto> OrderLines { get; private set; }

    public class OrderLineIntegrationEventDto
    {
        public OrderLineIntegrationEventDto(int itemId, decimal itemPrice, int quantity)
        {
            ItemId = itemId;
            ItemPrice = itemPrice;
            Quantity = quantity;
        }

        public int ItemId { get; private set; }

        public decimal ItemPrice { get; private set; }

        public int Quantity { get; private set; }
    }
}
