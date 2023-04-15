using SharedKernel;

namespace MSA.Template.Core.OrderAggregate.Specifications;

public sealed class RecentOrdersByPlacedStatusSpecification : BaseSpecification<Order>
{
    public RecentOrdersByPlacedStatusSpecification()
        : base(o => o.OrderStatus == OrderStatus.Placed)
    {
        AddInclude(o => o.OrderLines);
        ApplyOrderByDescending(o => o.OrderDate);
    }
}
