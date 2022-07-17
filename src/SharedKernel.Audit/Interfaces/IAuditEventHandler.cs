using MassTransit;

namespace SharedKernel.Audit.Interfaces;

public interface IAuditEventHandler<in T> : IConsumer<Batch<T>>
    where T : BaseAuditEvent
{
    
}