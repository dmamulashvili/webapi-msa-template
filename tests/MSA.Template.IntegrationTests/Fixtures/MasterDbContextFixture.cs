using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using MSA.Template.API.Services;
using MSA.Template.Infrastructure;
using SharedKernel.Audit.Interfaces;
using SharedKernel.Interfaces;

namespace MSA.Template.IntegrationTests.Fixtures;

public class MasterDbContextFixture : IDisposable
{
    public MasterDbContextFixture()
    {
        var dbOptions = new DbContextOptionsBuilder<MasterDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=Test_MSA.TemplateDb;User Id=postgres;password=postgres")
            .Options;

        var mediator = new Mock<IMediator>();
        var identityService = new IdentityService();
        ((IIdentityServiceProvider)identityService).SetUserIdentity(Guid.NewGuid());
        var auditEventService = new Mock<IAuditEventService>();

        DbContext = new MasterDbContext(dbOptions, mediator.Object, identityService,
            auditEventService.Object);

        DbContext.Database.EnsureCreated();
    }

    public MasterDbContext DbContext { get; private set; }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
    }
}