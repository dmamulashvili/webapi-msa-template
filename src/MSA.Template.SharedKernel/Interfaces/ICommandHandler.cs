using MediatR;

namespace MSA.Template.SharedKernel.Interfaces;

public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : BaseCommand<TResponse>
{
}