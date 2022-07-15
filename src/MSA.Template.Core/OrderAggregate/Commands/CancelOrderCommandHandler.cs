using Microsoft.Extensions.Logging;
using MSA.Template.SharedKernel;
using MSA.Template.SharedKernel.Exceptions;
using MSA.Template.SharedKernel.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.Core.OrderAggregate.Commands;

public class CancelOrderCommandHandler : BaseCommandHandler<CancelOrderCommand, bool>
{
    private readonly IRepository<Order, Guid> _repository;

    private readonly ILogger<PlaceOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(IRepository<Order, Guid> repository,
        ILogger<PlaceOrderCommandHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
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