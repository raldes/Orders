# Resilience side-effects

1. Is it possible that even when order service responds to a request with a 201 status code response the api client sends the same request again -for example as part of a retry policy on the api client side-?

Yes, it is possible, and the API must be idempotent.

An idempotent method is a method that can be called multiple times and get the same outcome. It would not matter if the method is called only once, or n times over. The result should be the same (the result refers to the real outcome of the method, that is, the persistence of data). 

The POST HTTP method (create entity) is not idempotent, because we could not create the entity more than once time. Other HTTP methods, like GET and PUT, are idempotent. 

Concerning the outcome to the caller, the method could look like idempotent, if that were required. We could register an idempotent key during creation and always respond the same result to the caller (this could be implemented in a gateway).

# From the message producer standpoint

2. Is it possible to publish messages in a different order than the one in which their respective orders were 
processed and persisted? If so, how can we avoid it? What trade-offs are considered?

3. Is it possible to publish message duplicates?

Yes, both are possible.

The asynchronous operations of the service and the sequential nature of the message broker do not allow, generally, to avoid those things occurring. But the service producer should add some additional information to the message so that the consumers can deal with those problems.

For example, the producer could add the timestamp of the original event, and also the Id of the transaction when was generated (and of course, the message content have the unique ID of the entity, too)

# From the message consumer standpoint

4. Is it possible to consume messages out of order? If so, how can we avoid it?

5. Is it possible to consume message duplicates? If so, how can we avoid it?

Yes, both are possible.

With the timestamp, the consumer could create an internal queue to process the messages in the right order for an entity Id.

With the transaction ID, the consumer could know if the message was duplicated.

# Message semantics

At the moment one of OrderCreatedEvent messages consumers is shipping service, which reacts to those messages 
creating order shipments accordingly. In case we decide to produce ShipOrderCommand messages instead of 
OrderConfirmedEvent ones,

6. (and 7)  Which are the semantic differences? Does service interaction change in a meaningful way?

The ShipOrderCommand is not the right name for that message. The correct name of a message must be a past verb related to the event that occurred and not the further action to do with it. (Think that others services could consume this message and do others different actions).

8.  Is there a need to introduce additional components or change system topology?

Change the message name (if the new name were ben right name) not should change the system topology.  The nature of the event-driven system will not change with the new topic.  But a new consumer should be created for that topic while the old one exists.