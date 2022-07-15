using MSA.Template.Core.OrderAggregate.Events;
using MSA.Template.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSA.Template.Core.OrderAggregate;

public class Order : BaseAggregateRoot<Guid>
{
    public Order(OrderStatus orderStatus, DateTimeOffset orderDate, Address address)
    {
        OrderStatus = orderStatus;
        OrderDate = orderDate;
        Address = address;
    }

    public OrderStatus OrderStatus { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public Address Address { get; private set; }

    private readonly List<OrderLine> _orderLines = new List<OrderLine>();
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines;

    public void AddOrderLine(int itemId, decimal itemPrice, int quantity)
    {
        var existingOrderLine = _orderLines.SingleOrDefault(o => o.ItemId == itemId);

        if (existingOrderLine != null)
        {
            existingOrderLine.AddQuantity(quantity);
        }
        else
        {
            var orderLine = new OrderLine(itemId, itemPrice, quantity);
            _orderLines.Add(orderLine);
        }
    }

    public void MarkAsPlaced()
    {
        if (OrderStatus == OrderStatus.Draft)
        {
            AddDomainEvent(new OrderPlacedDomainEvent(this));

            OrderStatus = OrderStatus.Placed;
        }
    }

    public void MarkAsPaid()
    {
        if (OrderStatus == OrderStatus.Placed)
        {
            AddDomainEvent(new OrderPaidDomainEvent(this));

            OrderStatus = OrderStatus.Paid;
        }
    }

    public void MarkAsShipped()
    {
        if (OrderStatus == OrderStatus.Paid)
        {
            OrderStatus = OrderStatus.Shipped;
        }
    }
    
    public void MarkAsCanceled()
    {
        if (OrderStatus != OrderStatus.Shipped)
        {
            OrderStatus = OrderStatus.Canceled;
        }
    }

    public static Order CreateNewDraft(Address address)
    {
        var order = new Order(OrderStatus.Draft, DateTimeOffset.UtcNow, address);
        return order;
    }
}