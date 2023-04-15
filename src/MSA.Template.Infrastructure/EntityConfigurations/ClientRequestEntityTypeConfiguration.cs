using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSA.Template.Infrastructure.Idempotency;

namespace MSA.Template.Infrastructure.EntityConfigurations;

class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
{
    public void Configure(EntityTypeBuilder<ClientRequest> requestConfiguration)
    {
        requestConfiguration.ToTable(nameof(ClientRequest), MasterDbContext.DefaultSchema);
        requestConfiguration.HasKey(request => request.Id);
        requestConfiguration.Property(request => request.Name).IsRequired();
        requestConfiguration.Property(request => request.Time).IsRequired();
    }
}