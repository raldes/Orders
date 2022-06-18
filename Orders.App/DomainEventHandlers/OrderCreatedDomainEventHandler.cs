using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Domain.Events;

namespace Orders.App.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly ILoggerFactory _logger;
        //private readonly IBus _bus;

        public OrderCreatedDomainEventHandler(
            ILoggerFactory logger)
            //IBus bus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task Handle(OrderCreatedDomainEvent orderCreatedDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<OrderCreatedDomainEvent>()
                .LogTrace($"Order with Id: {orderCreatedDomainEvent.Id} has been successfully creted");

            //done: publish event:
            //_bus.Publish(orderCreatedDomainEvent);
        }
    }
}