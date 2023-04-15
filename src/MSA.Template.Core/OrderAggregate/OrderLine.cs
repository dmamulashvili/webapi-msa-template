using SharedKernel;

namespace MSA.Template.Core.OrderAggregate;

public class OrderLine : BaseEntity<int>
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

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }
}
