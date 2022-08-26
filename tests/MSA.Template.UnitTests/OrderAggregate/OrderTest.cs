using MSA.Template.Core.OrderAggregate;
using MSA.Template.Core.OrderAggregate.Events;
using SharedKernel.Exceptions;

namespace MSA.Template.UnitTests.OrderAggregate;

public class OrderTest
{
    [Fact]
    public void CreateNewDraft_Creates_Order_With_OrderStatus_Draft()
    {
        var address = CreateEmptyAddress();

        var order = Order.CreateNewDraft(address);

        Assert.Equal(OrderStatus.Draft, order.OrderStatus);
    }

    [Fact]
    public void CreateNewDraft_Creates_Order_With_OrderDate_DateTimeOffset_UtcNow()
    {
        var address = CreateEmptyAddress();

        var order = Order.CreateNewDraft(address);

        Assert.Equal(DateTimeOffset.UtcNow.DateTime, order.OrderDate.DateTime, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddOrderLine_Adds_OrderLine_To_OrderLines_List()
    {
        var order = CreateDraftOrder();
        
        var orderLine = new OrderLine(1, 1, 1);

        order.AddOrderLine(orderLine);

        Assert.Equal(orderLine, order.OrderLines.Single());
    }

    [Fact]
    public void MarkAsPlaced_Changes_OrderStatus_To_Placed()
    {
        var order = CreateDraftOrder();

        order.MarkAsPlaced();

        Assert.Equal(OrderStatus.Placed, order.OrderStatus);
    }

    [Fact]
    public void MarkAsPlaced_Raises_Single_OrderPlacedDomainEvent()
    {
        var order = CreateDraftOrder();

        order.MarkAsPlaced();

        Assert.Single(order.DomainEvents!);
        Assert.IsType<OrderPlacedDomainEvent>(order.DomainEvents!.First());
    }

    [Fact]
    public void MarkAsPlaced_Throws_DomainException_If_OrderStatus_Is_Not_Draft()
    {
        var order = CreatePlacedOrder();

        void Act() => order.MarkAsPlaced();

        Assert.Throws<DomainException>(Act);
    }

    private Order CreateDraftOrder()
    {
        var address = CreateEmptyAddress();
        return new Order(OrderStatus.Draft, DateTimeOffset.UtcNow, address);
    }

    private Order CreatePlacedOrder()
    {
        var address = CreateEmptyAddress();
        return new Order(OrderStatus.Placed, DateTimeOffset.UtcNow, address);
    }

    private Address CreateEmptyAddress()
    {
        return new Address(string.Empty, string.Empty);
    }
}