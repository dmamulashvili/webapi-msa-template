using SharedKernel;
using System;
using System.Collections.Generic;

namespace IntegrationEvents;

public class OrderPlacedIntegrationEvent : BaseIntegrationEvent
{
    public OrderPlacedIntegrationEvent(Guid orderId, DateTimeOffset orderDate,
        IReadOnlyCollection<OrderLine> orderLines)
    {
        OrderId = orderId;
        OrderDate = orderDate;
        OrderLines = orderLines;
    }

    public Guid OrderId { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public IReadOnlyCollection<OrderLine> OrderLines { get; private set; }


    public class OrderLine
    {
        public OrderLine(int itemId, decimal itemPrice, int quantity)
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