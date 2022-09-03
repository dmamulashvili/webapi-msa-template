using MSA.Template.Core.OrderAggregate;

namespace MSA.Template.UnitTests.Builders;

public class OrderBuilder
{
    public Order CreateDraftOrder()
    {
        var address = CreateEmptyAddress();
        return new Order(OrderStatus.Draft, DateTimeOffset.UtcNow, address);
    }

    public Order CreatePlacedOrder()
    {
        var address = CreateEmptyAddress();
        return new Order(OrderStatus.Placed, DateTimeOffset.UtcNow, address);
    }

    public Address CreateEmptyAddress()
    {
        return new Address(string.Empty, string.Empty);
    }
}