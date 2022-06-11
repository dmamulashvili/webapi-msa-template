using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MSA.Template.SharedKernel;
using MSA.Template.SharedKernel.Exceptions;
using MSA.Template.SharedKernel.Extensions;
using MSA.Template.SharedKernel.Interfaces;

namespace MSA.Template.Core;

/// <summary>
/// Provides a base implementation for handling duplicate request and ensuring idempotent updates, in the cases where
/// a requestid sent by client is used to detect duplicate requests.
/// </summary>
/// <typeparam name="TRequest">Type of the command handler that performs the operation if request is not duplicated</typeparam>
/// <typeparam name="TResponse">Return value of the inner command handler</typeparam>
public class
    IdentifiedCommandHandler<TRequest, TResponse> : IRequestHandler<IdentifiedCommand<TRequest, TResponse>, TResponse>
    where TRequest : BaseCommand<TResponse>
{
    private readonly IMediator _mediator;
    private readonly IRequestManager _requestManager;
    private readonly ILogger<IdentifiedCommandHandler<TRequest, TResponse>> _logger;

    public IdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<TRequest, TResponse>> logger)
    {
        _mediator = mediator;
        _requestManager = requestManager;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
    /// just enqueues the original inner command.
    /// </summary>
    /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Return value of inner command or default value if request same ID was found</returns>
    public async Task<TResponse> Handle(IdentifiedCommand<TRequest, TResponse> message,
        CancellationToken cancellationToken)
    {
        var alreadyExists = await _requestManager.ExistAsync(message.CorrelationId);
        if (alreadyExists)
        {
            throw new DuplicateRequestException();
        }

        await _requestManager.CreateRequestForCommandAsync<TRequest>(message.CorrelationId);

        var command = message.Command;
        var commandName = command.GetGenericTypeName();

        message.Command.CorrelationId = message.CorrelationId;
        var idProperty = nameof(message.Command.CorrelationId);
        var commandId = message.Command.CorrelationId;

        _logger.LogInformation(
            "----- Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            commandName,
            idProperty,
            commandId,
            command);

        // Send the embeded business command to mediator so it runs its related CommandHandler 
        var result = await _mediator.Send(command, cancellationToken);

        _logger.LogInformation(
            "----- Command result: {@Result} - {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            result,
            commandName,
            idProperty,
            commandId,
            command);

        return result;
    }
}