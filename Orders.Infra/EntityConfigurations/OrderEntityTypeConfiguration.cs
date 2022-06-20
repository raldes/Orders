using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Orders.Domain.Entities;
using Orders.Infra.Database;

namespace Orders.Infra.EntityConfigurations;

class OrderEntityTypeConfiguration : IEntityTypeConfiguration<OrderAggregateRoot>
{
    public void Configure(EntityTypeBuilder<OrderAggregateRoot> orderConfiguration)
    {
        orderConfiguration.ToTable("orders", OrdersDbContext.DEFAULT_SCHEMA);
        
        orderConfiguration.HasKey(o => o.Id);

        orderConfiguration
            .Property(p => p.Items)
            .HasConversion(v => JsonConvert.SerializeObject(v),
                           v => JsonConvert.DeserializeObject<IDictionary<string, decimal>>(v));

        orderConfiguration.Ignore(b => b.DomainEvents);

        orderConfiguration.Property<string>("Description").IsRequired(false);

    }
}
