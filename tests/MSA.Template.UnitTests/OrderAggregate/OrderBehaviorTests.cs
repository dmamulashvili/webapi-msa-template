using MSA.Template.Core.OrderAggregate;
using MSA.Template.Core.OrderAggregate.Events;
using MSA.Template.UnitTests.Builders;
using SharedKernel.Exceptions;

namespace MSA.Template.UnitTests.OrderAggregate;

public class OrderBehaviorTests
{
    private OrderBuilder OrderBuilder { get; } = new();

    [Fact]
    public void AddOrderLine_Adds_OrderLine_To_OrderLines_List()
    {
        var order = OrderBuilder.CreateDraftOrder();

        var orderLine = new OrderLine(1, 1, 1);

        order.AddOrderLine(orderLine);

        Assert.Equal(orderLine, order.OrderLines.First());
    }

    [Fact]
    public void MarkAsPlaced_Changes_OrderStatus_To_Placed()
    {
        var order = OrderBuilder.CreateDraftOrder();

        order.MarkAsPlaced();

        Assert.Equal(OrderStatus.Placed, order.OrderStatus);
    }

    [Fact]
    public void MarkAsPlaced_Raises_Single_OrderPlacedDomainEvent()
    {
        var order = OrderBuilder.CreateDraftOrder();

        order.MarkAsPlaced();

        Assert.Single(order.DomainEvents!);
        Assert.IsType<OrderPlacedDomainEvent>(order.DomainEvents!.First());
    }

    [Fact]
    public void MarkAsPlaced_Throws_DomainException_If_OrderStatus_Is_Not_Draft()
    {
        var order = OrderBuilder.CreatePlacedOrder();

        void Act() => order.MarkAsPlaced();

        Assert.Throws<DomainException>(Act);
    }
}