using Microsoft.Extensions.DependencyInjection;
using MSA.Template.Audit.Interfaces;
using MSA.Template.Audit.Services;

namespace MSA.Template.Audit;

public static class DependencyInjection
{
    public static IServiceCollection AddAuditUsingMassTransit(this IServiceCollection services)
    {
        services.AddScoped<IAuditEventService, AuditEventService>();

        return services;
    }
}