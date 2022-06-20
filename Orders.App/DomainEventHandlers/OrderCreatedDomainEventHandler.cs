using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.App.IntegrationEvents;
using Orders.App.IntegrationEvents.Events;
using Orders.Domain.Events;

namespace Orders.App.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        //private readonly IMediator _mediator;

        private readonly ILoggerFactory _logger;
        //private readonly IBus _bus;

        public OrderCreatedDomainEventHandler(
            ILoggerFactory logger,
            //IMediator mediator,
            IOrderingIntegrationEventService orderingIntegrationEventService
            //IBus bus
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));

            //_bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task Handle(OrderCreatedDomainEvent orderCreatedDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<OrderCreatedDomainEvent>()
                .LogTrace($"Order with Id: {orderCreatedDomainEvent.Id} has been successfully creted");

            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent(orderCreatedDomainEvent.Id);

            await _orderingIntegrationEventService.AddAndSaveEventAsync(orderCreatedIntegrationEvent);


            //done: publish event:
            //_bus.Publish(orderCreatedDomainEvent);
        }
    }
}