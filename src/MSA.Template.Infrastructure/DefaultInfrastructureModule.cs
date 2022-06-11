using Autofac;
using MSA.Template.Infrastructure.Data;
using MSA.Template.Infrastructure.Idempotency;
using MSA.Template.IntegrationEvents.Services;
using MSA.Template.SharedKernel.IntegrationEvents;
using MSA.Template.SharedKernel.Interfaces;

namespace MSA.Template.Infrastructure;

public class DefaultInfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(BaseRepository<,>))
            .As(typeof(IRepository<,>))
            .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(BaseReadOnlyRepository<,>))
            .As(typeof(IReadOnlyRepository<,>))
            .InstancePerLifetimeScope();

        builder
            .RegisterType<RequestManager>()
            .As<IRequestManager>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<IntegrationEventService>()
            .As<IIntegrationEventService>()
            .InstancePerLifetimeScope();
    }
}