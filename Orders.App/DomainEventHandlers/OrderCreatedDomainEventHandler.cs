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

        private readonly ILoggerFactory _logger;

        public OrderCreatedDomainEventHandler(
            ILoggerFactory logger,
            IOrderingIntegrationEventService orderingIntegrationEventService
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));

        }

        public async Task Handle(OrderCreatedDomainEvent orderCreatedDomainEvent, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<OrderCreatedDomainEvent>()
                .LogTrace($"Order with Id: {orderCreatedDomainEvent.Id} has been successfully creted");

            var orderCreatedIntegrationEvent = new OrderCreatedIntegrationEvent(orderCreatedDomainEvent.Id);

            await _orderingIntegrationEventService.AddAndSaveEventAsync(orderCreatedIntegrationEvent);
        }
    }
}