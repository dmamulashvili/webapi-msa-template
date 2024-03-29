using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class CancelOrderCommandHandler : BaseCommandHandler<CancelOrderCommand, bool>
{
    private readonly IRepository<Order, Guid> _repository;

    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(
        IRepository<Order, Guid> repository,
        ILogger<PlaceOrderCommandHandler> logger
    )
    {
        _repository = Guard.Against.Null(repository);
        _logger = Guard.Against.Null(logger);
    }

    public override async Task<bool> Handle(
        CancelOrderCommand command,
        CancellationToken cancellationToken
    )
    {
        var order = await _repository.FindByIdAsync(command.Id);

        if (order is null)
        {
            throw new DomainException("Order not found.");
        }

        order.MarkAsCanceled();

        _logger.LogInformation("----- Canceling Order - Order: {@Order}", order);

        return await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
