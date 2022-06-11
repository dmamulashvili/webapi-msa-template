using MSA.Template.SharedKernel;
using System;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class MarkOrderAsPaidCommand : BaseCommand<bool>
{
    public MarkOrderAsPaidCommand(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}