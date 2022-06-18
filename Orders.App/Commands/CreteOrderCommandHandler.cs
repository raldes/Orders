using Light.GuardClauses;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.App.IntegrationEvents;
using Orders.App.IntegrationEvents.Events;
using Orders.Domain.Commands;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Repositories;

namespace Orders.App.Commands
{
    public class CreteOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
 
        private readonly IEFRepository<OrderAggregateRoot> _repository;
        private readonly ILogger<CreteOrderCommandHandler> _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly IMediator _mediator;

        public CreteOrderCommandHandler(
            IEFRepository<OrderAggregateRoot> repository,
            ILogger<CreteOrderCommandHandler> logger,
            IMediator mediator,
            IOrderingIntegrationEventService orderingIntegrationEventService)
        
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository)); ;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
        }

        public async Task<bool> Handle(CreateOrderCommand createOrderCommand, CancellationToken cancellationToken)
        {
            // TODO: atomically instantiate a new OrderAggregateRoot, persist it and send OrderCreatedEvent

            try
            {
                // Add/Update the AggregateRoot
                // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
                // methods and constructor so validations, invariants and business logic 
                // make sure that consistency is preserved across the whole aggregate
                var order = new OrderAggregateRoot(createOrderCommand);

                _logger.LogInformation($"----- Creating Order - Order: {order.Id}");

                _repository.Add(order);

                var saveResult = await _repository.UnitOfWork
                    .SaveEntitiesAsync(cancellationToken);

                if(saveResult)
                {
                    //if successfull db transaction: send OrderCreatedDomainEvent :

                    var createdOrderEvent = new OrderCreatedDomainEvent(order.Id, order.Items);

                    await _mediator.Publish(createdOrderEvent);


                    //............

                    //or send OrderCreatedIntegrationEvent to other services:

                    //var orderStartedIntegrationEvent = new OrderCreatedIntegrationEvent(order.Id);

                    //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStartedIntegrationEvent);

                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
