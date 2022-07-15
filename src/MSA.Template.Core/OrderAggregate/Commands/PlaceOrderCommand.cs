using System.Collections.Generic;
using System.Runtime.Serialization;
using MSA.Template.SharedKernel;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class PlaceOrderCommand : BaseCommand<bool>
{

    public PlaceOrderCommand(string city, string street, IEnumerable<OrderLineDto> orderLines)
    {
        City = city;
        Street = street;
        OrderLines = orderLines;
    }

    public string City { get; private set; }

    public string Street { get; private set; }

    public IEnumerable<OrderLineDto> OrderLines { get; private set; }

    public record OrderLineDto
    {
        public int ItemId { get; init; }

        public int Quantity { get; init; }
    }
}