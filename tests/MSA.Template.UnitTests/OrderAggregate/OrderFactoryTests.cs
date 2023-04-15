using MSA.Template.Core.OrderAggregate;
using MSA.Template.UnitTests.Builders;

namespace MSA.Template.UnitTests.OrderAggregate;

public class OrderFactoryTests
{
    private OrderBuilder OrderBuilder { get; } = new();

    [Fact]
    public void CreateNewDraft_Creates_Order_With_OrderStatus_Draft()
    {
        var address = OrderBuilder.CreateEmptyAddress();

        var order = Order.CreateNewDraft(address);

        Assert.Equal(OrderStatus.Draft, order.OrderStatus);
    }

    [Fact]
    public void CreateNewDraft_Creates_Order_With_OrderDate_DateTimeOffset_UtcNow()
    {
        var address = OrderBuilder.CreateEmptyAddress();

        var order = Order.CreateNewDraft(address);

        Assert.Equal(
            DateTimeOffset.UtcNow.DateTime,
            order.OrderDate.DateTime,
            TimeSpan.FromSeconds(1)
        );
    }
}
