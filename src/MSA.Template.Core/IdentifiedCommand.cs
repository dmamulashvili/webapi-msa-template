using System;
using MediatR;

namespace MSA.Template.Core;

public class IdentifiedCommand<T, R> : IRequest<R>
    where T : IRequest<R>
{
    public T Command { get; }
    public Guid CorrelationId { get; }
    public bool SkipIdempotency { get; }

    public IdentifiedCommand(T command, Guid correlationId, bool skipIdempotency = false)
    {
        Command = command;
        CorrelationId = correlationId;
        SkipIdempotency = skipIdempotency;
    }
}
