using MediatR;

namespace Orders.Domain.Commands
{ 
    public class CreateOrderCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public IDictionary<string, decimal> Items { get; set; }
    }
}
