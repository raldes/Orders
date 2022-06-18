# Orders

## Solution

- The solution has 8 projects
- Al projects are .Net 6

### Projects: 

- Orders.Api: Api REST with Swagger
- Orders.App: Application services and Automapper profiles (class lib)
- Orders.Domain: Domain and DTOs (class lib)
- Order.Infra: Db Context and Generic EF Repository (class lib)

As building blocks:
- EventBus
- EventBusRabbitMQ
- IntegrationEventLogEF


#### DomaineEntities

- OrderAggregateRoot

#### Database

- The Infrastructure uses EF Core Postgres provider. 

