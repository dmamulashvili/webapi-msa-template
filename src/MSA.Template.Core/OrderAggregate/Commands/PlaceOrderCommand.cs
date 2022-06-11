using System.Collections.Generic;
using System.Runtime.Serialization;
using MSA.Template.SharedKernel;

namespace MSA.Template.Core.OrderAggregate.Commands;

[DataContract]
public class PlaceOrderCommand : BaseCommand<bool>
{

    public PlaceOrderCommand(string city, string street, IEnumerable<OrderItemDTO> orderItems)
    {
        City = city;
        Street = street;
        OrderItems = orderItems;
    }

    [DataMember] public string City { get; private set; }

    [DataMember] public string Street { get; private set; }

    [DataMember] public IEnumerable<OrderItemDTO> OrderItems { get; private set; }

    public record OrderItemDTO
    {
        public int ItemId { get; init; }

        public int Quantity { get; init; }
    }
}