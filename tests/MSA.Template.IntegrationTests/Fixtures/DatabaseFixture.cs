using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using MSA.Template.API.Services;
using MSA.Template.Infrastructure;
using SharedKernel.Audit.Interfaces;
using SharedKernel.Interfaces;

namespace MSA.Template.IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        var dbOptions = new DbContextOptionsBuilder<MasterDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=Test_MSA.TemplateDb;User Id=postgres;password=postgres")
            .Options;

        var mediator = Mock.Of<IMediator>();
        var identityService = new IdentityService();
        ((IIdentityServiceProvider)identityService).SetUserIdentity(Guid.NewGuid());
        var auditEventService = Mock.Of<IAuditEventService>();

        MasterDbContext = new MasterDbContext(dbOptions, mediator, identityService,
            auditEventService);
        SlaveDbContext = new SlaveDbContext(dbOptions);

        MasterDbContext.Database.EnsureCreated();
    }

    public MasterDbContext MasterDbContext { get; }
    public SlaveDbContext SlaveDbContext { get; }

    public void Dispose()
    {
        MasterDbContext.Database.EnsureDeleted();
    }
}