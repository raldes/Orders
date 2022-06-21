
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

        public static OrderAggregateRoot CreateOrder(CreateOrderCommand command)
        {
            var newOrder = new OrderAggregateRoot(command);

            newOrder.Apply(command);

            var createdOrderEvent = new OrderCreatedDomainEvent(newOrder.Id, newOrder.Items);

            //generate domain event:
            newOrder.AddDomainEvent(createdOrderEvent);

            return newOrder;
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
