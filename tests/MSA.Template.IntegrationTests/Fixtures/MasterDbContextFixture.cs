using MediatR;
using Microsoft.EntityFrameworkCore;
using MSA.Template.API.Services;
using MSA.Template.Infrastructure;
using SharedKernel.Interfaces;

namespace MSA.Template.IntegrationTests.Fixtures;

public class MasterDbContextFixture : IDisposable
{
    public MasterDbContextFixture()
    {
        var dbOptions = new DbContextOptionsBuilder<MasterDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=TestMSA.TemplateDb;User Id=postgres;password=postgres")
            .Options;
        
        var identityService = new IdentityService();
        ((IIdentityServiceProvider)identityService).SetUserIdentity(Guid.NewGuid());
        
        DbContext = new MasterDbContext(dbOptions, new Mediator(null!), identityService,
            new AuditEventService(null!, null!));

        DbContext.Database.EnsureCreated();
    }

    public MasterDbContext DbContext { get; private set; }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
    }
}