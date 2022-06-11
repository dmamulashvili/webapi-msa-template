using MSA.Template.Core.OrderAggregate.Events;
using MSA.Template.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSA.Template.Core.OrderAggregate;

public class Order : BaseAggregateRoot<Guid>
{
    public Order(Guid userId, OrderStatus orderStatus, DateTimeOffset orderDate, Address address)
    {
        UserId = userId;
        OrderStatus = orderStatus;
        OrderDate = orderDate;
        Address = address;
    }

    public Guid UserId { get; private set; }

    public OrderStatus OrderStatus { get; private set; }

    public DateTimeOffset OrderDate { get; private set; }

    public Address Address { get; private set; }

    private readonly List<OrderItem> _orderItems = new List<OrderItem>();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public void AddOrderItem(int itemId, decimal itemPrice, int quantity)
    {
        var existingOrderItem = _orderItems.SingleOrDefault(o => o.ItemId == itemId);

        if (existingOrderItem != null)
        {
            existingOrderItem.AddQuantity(quantity);
        }
        else
        {
            var orderItem = new OrderItem(itemId, itemPrice, quantity);
            _orderItems.Add(orderItem);
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

    public static Order CreateNewDraft(Guid userId, Address address)
    {
        var order = new Order(userId, OrderStatus.Draft, DateTimeOffset.UtcNow, address);
        return order;
    }
}