using System;
using System.Linq;
using MSA.Template.SharedKernel;

namespace MSA.Template.Core.OrderAggregate.Specifications;

public sealed class OrdersByUserIdSpecification : BaseSpecification<Order>
{
    public OrdersByUserIdSpecification(Guid userId) : base(o => o.UserId == userId)
    {
        AddInclude(o => o.OrderItems);
        ApplyOrderByDescending(o => o.OrderDate);
    }
}