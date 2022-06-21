# Orders

## Solution

- The solution has 6 projects
- Al projects are .Net 6

### Projects: 

- Orders.Api: Api REST with Swagger
- Orders.App: Application services and Automapper profiles (class lib)
- Orders.Domain: Domain and DTOs (class lib)
- Order.Infra: Db Context and Generic EF Repository (class lib)

As building blocks:
- EventBus
- EventBusRabbitMQ

#### DomaineEntities

- OrderAggregateRoot

#### Database

- The Infrastructure uses: EF Core Postgres database provider and RabbitMQ service bus for integration events.

### Integration events for sharing events between services.	

- Integration Events is base type for all integration events.

- IEventBus is an interface for Publish and Subscribe Integration events.

- IntegrationEventLogEntry register the integration events that are published.

- DefaultRabbitMQPersistentConnection: 
Create Persistent Connection for Rabbit. Uses Polly for retry N attemps.

### Run the application

- Needed Infrastructure:

In order to facilitate the application test, we use free online 
accounts for Postgres database and rabbitMQ service bus. 
Please replace the supply postgres connection string and rabbit endpoint in appsettings.json.

#### Postgres database : the application uses orderdb database Postgres for storage the Order
 and IntegrationEventLogEntry entries.

#### RabbitMq service bus: the application uses RabbitMQ to publish IntegrationEvents
