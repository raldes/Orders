# Orders

## Solution

This solution is DDD, CQRS, and EventDriven based.

For commands the controller sends Commands to CommandHandler methods (in the App layer), using MediatR.

The command handler calls de entities methods (in the domain layer) to create the entity.

Aggregate root: only the entity (in the domain layer) contains the logic to create and modify itself, modify its status and generate DomainEvents related to these modifications.

The Command handler call repositories to persist the changes. The repository uses UnitOfWorks pattern to persist the changes only if all the db operations were successful (transaction mode).

The DomainEventHandler sends IntegrationEvent to communicate other services about that event (EventDriven)

Any other service could be to subscribe IntegrationEvents to know about these events.


## Projects: 

The solution has 6 projects. All projects are .Net 6

- Orders.Api: Api REST with Swagger
- Orders.App: Application services (class lib)
- Orders.Domain: Domain and DTOs (class lib)
- Order.Infra: Db Context and Generic EF Repository (class lib)

As building blocks:
- EventBus
- EventBusRabbitMQ

## Designing atomicity and resiliency when publishing to the event bus 

(from NET-Microservices-Architecture-for-Containerized-NET-Applications-(Microsoft-eBook)

"When you publish integration events through a distributed messaging system like your event bus, you have the problem of atomically updating the original database and publishing an event (that is, either both operations complete or none of them)... The CAP theorem says that you cannot build a (distributed) database (or a microservice that owns its model) that is continually available, strongly consistent, and tolerant to any partition. You must choose two of these three properties...In microservices-based architectures, you should choose availability and tolerance, and you should deemphasize strong consistency." 

We can have several approaches for dealing with data and event consistency:
- Using the full Event Sourcing pattern.
- Using transaction log mining.
- Using the Outbox pattern. This is a transactional table to store the integration events (extending the local transaction)."

In the solution, we use a balanced approach: a transactional database table and a simplified ES pattern. We have an event state property, the estate “ready to publish the event,” is set in the original event when committing it to the integration events table. Then try to publish the event on the event bus. If the publish-event action succeeds, start another transaction in the origin service and move the state from “ready to publish the event” to “event already published.”

If the publish-event action in the event bus fails, the data still will not be inconsistent within the origin microservice—it is still marked as “ready to publish the event,” and for the rest of the services, it will eventually be consistent. 

![Image](/pictures/publish2.png)

We also can have background jobs checking the state of the transactions or integration events. If the job finds an event in the “ready to publish the event” state, it can try to republish that event to the event bus.

![Image](/pictures/publish3.png)

### Implementing Integration events for sharing events between services.	

- IntegrationEvent is the base type for all integration events.

- IEventBus is the interface for Publish and Subscribe Integration events.

- IntegrationEventLogEntry is the record of the integration events that are published.

### Resiliance

- DefaultRabbitMQPersistentConnection:  Create Persistent Connection for Rabbit. Uses Polly for retry N attempts.

### Postgres Database and RabbitMQ service bus.

- The Infrastructure project uses EF Core and Postgres database provider. The application uses orderdb database Postgres for storage the Order and IntegrationEventLogEntry entries.

- The application uses RabbitMQ to publish IntegrationEvents

### Run the application

The application has a docker file for deploying it.

To facilitate the application test, we use free online accounts for Postgres database and rabbitMQ service bus. 

Note: To run the application please ask for the credentials and use them in Postgres parameters and rabbit endpoint in appsettings.json.
