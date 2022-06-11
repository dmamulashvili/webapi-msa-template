using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSA.Template.Core.OrderAggregate;

namespace MSA.Template.Infrastructure.EntityConfigurations;

public class OrderEntityTypeConfiguration: IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(nameof(Order), MasterDbContext.DefaultSchema);
        builder.HasKey(order => order.Id);
        builder.Property(order => order.Address).HasColumnType("jsonb");
    }
}