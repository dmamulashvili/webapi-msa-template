using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSA.Template.Core.OrderAggregate;

namespace MSA.Template.Infrastructure.EntityConfigurations;

public class OrderLineEntityTypeConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable(nameof(OrderLine));
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.ItemId);
    }
}