using MediatR;

namespace Orders.Domain.Events
{
    public class OrderCreatedDomainEvent : INotification
    {
        public Guid Id { get; set; }
        public IDictionary<string, decimal> Items { get; set; }

        public OrderCreatedDomainEvent(Guid id, IDictionary<string, decimal> items )
        {
            Id = id;
            Items = items;
        }
    }
}