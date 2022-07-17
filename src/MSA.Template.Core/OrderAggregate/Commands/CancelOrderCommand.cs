using SharedKernel;
using System;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class CancelOrderCommand : BaseCommand<bool>
{
    public CancelOrderCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; private set; }
}