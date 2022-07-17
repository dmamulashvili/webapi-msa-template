using MediatR;

namespace SharedKernel.Interfaces;

public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : BaseCommand<TResponse>
{
}