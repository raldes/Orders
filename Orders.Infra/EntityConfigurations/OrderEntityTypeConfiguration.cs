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

        //orderConfiguration.Property(o => o.Id)
        //    .UseHiLo("orderseq", OrdersDbContext.DEFAULT_SCHEMA);
 
        orderConfiguration.Property<string>("Description").IsRequired(false);

        var navigation = orderConfiguration.Metadata.FindNavigation(nameof(OrderAggregateRoot.Items));

        // DDD Patterns comment:
        //Set as field (New since EF 1.1) to access the Item collection property through its field
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

    }
}
