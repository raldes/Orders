# Inventory

## Solution

- The solution has 5 projects
- Al projects are .Net 6

### Projects: 

- Orders.Api: Api REST with Swagger
- Orders.App: Application services and Automapper profiles (class lib)
- Orders.Domain: Domain and DTOs (class lib)
- Infrastructure: Db Context and Generic EF Repository (class lib)
- Orders.Web: Web App MVC (UI) - Deployed for UI testing

The projects Api and Web use the same services for entities management.

#### Entities

We have two entities: Item and ItemType. 
- Item: is the main entity. It has a foreign key to ItemTypes table.
- ItemType contains the types of items.

(See ER Diagram.png)

#### Database

- The Infrastructure uses EF Core InMemory provider. 
- When the application starts, some ItemTypes and Items are generated for testing purpose.
