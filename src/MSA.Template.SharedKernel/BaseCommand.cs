using System;
using MediatR;

namespace MSA.Template.SharedKernel;

public abstract class BaseCommand<TResponse> : IRequest<TResponse>
{
    public Guid CorrelationId { get; set; }
}