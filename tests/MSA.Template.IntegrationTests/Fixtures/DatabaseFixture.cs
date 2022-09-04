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

        var mediator = new Mock<IMediator>();
        var identityService = new IdentityService();
        ((IIdentityServiceProvider)identityService).SetUserIdentity(Guid.NewGuid());
        var auditEventService = new Mock<IAuditEventService>();

        MasterDbContext = new MasterDbContext(dbOptions, mediator.Object, identityService,
            auditEventService.Object);
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