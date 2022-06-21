using BuildingBlocks.EventBus.Events;
using System.Text.Json.Serialization;

namespace Orders.App.IntegrationEvents.Events;

// An Integration Event is an event that can cause side effects to other microservices, Bounded-Contexts or external systems.
public record OrderCreatedIntegrationEvent : IntegrationEvent
{
    [JsonInclude]
    public Guid OrderId { get; init; }

    public OrderCreatedIntegrationEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}
