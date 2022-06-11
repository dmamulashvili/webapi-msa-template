using MassTransit;

namespace MSA.Template.Audit.Interfaces;

public interface IAuditEventHandler<in T> : IConsumer<Batch<T>>
    where T : BaseAuditEvent
{
    
}