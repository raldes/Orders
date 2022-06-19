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
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
 
        private readonly IEFRepository<OrderAggregateRoot> _repository;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IEFRepository<OrderAggregateRoot> repository,
            ILogger<CreateOrderCommandHandler> logger)
        
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository)); ;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        //todo: create Postgres repo, with migration (rest.appt.providers)
        //pipeline exception and response if exception
        //rabbit config
        //docker compose with postgres rabbit

        public async Task<bool> Handle(CreateOrderCommand createOrderCommand, CancellationToken cancellationToken)
        {
            // TODO: atomically instantiate a new OrderAggregateRoot, persist it and send OrderCreatedEvent

            try
            {
                // Add/Update the AggregateRoot
                // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
                // methods and constructor so validations, invariants and business logic 
                // make sure that consistency is preserved across the whole aggregate

                //crete the entity and Created domain event 
                var order =  OrderAggregateRoot.CreateOrder(createOrderCommand);

                _logger.LogInformation($"----- Creating Order - Order: {order.Id}");

                _repository.Add(order);

                var saveResult = await _repository.UnitOfWork
                   .SaveEntitiesAsync(cancellationToken);

                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}


