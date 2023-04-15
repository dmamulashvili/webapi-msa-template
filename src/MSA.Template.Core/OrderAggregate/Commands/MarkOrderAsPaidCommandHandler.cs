using SharedKernel;
using SharedKernel.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class MarkOrderAsPaidCommandHandler : BaseCommandHandler<MarkOrderAsPaidCommand, bool>
{
    private readonly IRepository<Order, Guid> _repository;

    public MarkOrderAsPaidCommandHandler(IRepository<Order, Guid> repository)
    {
        _repository = Guard.Against.Null(repository);
    }

    public override async Task<bool> Handle(MarkOrderAsPaidCommand command, CancellationToken cancellationToken)
    {
        var order = await _repository.FindByIdAsync(command.Id);

        if (order is null)
        {
            return false;
        }

        order.MarkAsPaid();

        return await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}