using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using SharedKernel.Extensions;
using SharedKernel.IntegrationEvents;
using SharedKernel.Interfaces;
using SharedKernel.Audit.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.Core.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BaseCommand<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IIntegrationEventService _integrationEventService;
    private readonly IAuditEventService _auditEventService;

    public TransactionBehavior(
        ILogger<TransactionBehavior<TRequest, TResponse>> logger,
        IUnitOfWork unitOfWork,
        IIntegrationEventService integrationEventService,
        IAuditEventService auditEventService
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentException(nameof(IUnitOfWork));
        _integrationEventService =
            integrationEventService ?? throw new ArgumentException(nameof(integrationEventService));
        _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        _auditEventService = auditEventService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next
    )
    {
        var typeName = request.GetGenericTypeName();

        try
        {
            if (_unitOfWork.HasActiveTransaction)
            {
                return await next();
            }

            var strategy = _unitOfWork.CreateExecutionStrategy(request.CorrelationId);

            return await strategy.ExecuteAsync(async () =>
            {
                var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

                Guard.Against.Null(transaction, nameof(transaction));

                _logger.LogInformation(
                    "----- Begin transaction {TransactionId} for {CommandName} ({@Command})",
                    transaction.TransactionId,
                    typeName,
                    request
                );

                var response = await next();

                _logger.LogInformation(
                    "----- Commit transaction {TransactionId} for {CommandName}",
                    transaction.TransactionId,
                    typeName
                );

                await _integrationEventService.PublishEventsAsync(
                    request.CorrelationId,
                    cancellationToken
                );

                await _unitOfWork.CommitTransactionAsync(transaction);

                // TODO: To enable Outbox on AuditEvents move _auditEventService.PublishEventsAsync before _unitOfWork.CommitTransactionAsync. Also view AuditEventService.cs
                await _auditEventService.PublishEventsAsync(cancellationToken);

                return response;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "ERROR Handling transaction for {CommandName} ({@Command})",
                typeName,
                request
            );

            await _unitOfWork.RollbackTransaction();
            throw;
        }
    }
}
