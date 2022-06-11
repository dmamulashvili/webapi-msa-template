using MSA.Template.SharedKernel.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace MSA.Template.SharedKernel;

public abstract class BaseCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : BaseCommand<TResult>
{
    public abstract Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}