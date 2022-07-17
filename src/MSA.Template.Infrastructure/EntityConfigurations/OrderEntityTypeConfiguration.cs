using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSA.Template.Core.OrderAggregate;

namespace MSA.Template.Infrastructure.EntityConfigurations;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(nameof(Order), MasterDbContext.DefaultSchema);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Address).HasColumnType("jsonb");

        builder.HasMany(p => p.OrderLines).WithOne();
    }
}