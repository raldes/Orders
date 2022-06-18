
using Orders.Domain.Commands;
using Orders.Domain.Events;

namespace Orders.Domain.Entities
{
    public class OrderAggregateRoot : EFEntity
    {
        public Guid Id { get; set; }

        public IDictionary<string, decimal> Items { get; set; }

        public OrderAggregateRoot()
        {
        }

        public OrderAggregateRoot(CreateOrderCommand command)
        {
            this.Apply(command);
        }

        private void Apply(CreateOrderCommand command)
        {
            this.Id = command.Id;
            this.Items = command.Items;

        }
    }
}
