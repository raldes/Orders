namespace Orders.Domain.Exceptions;

public class CreateOrderDomainException : Exception
{
    public CreateOrderDomainException()
    { }

    public CreateOrderDomainException(string message)
        : base(message)
    { }

    public CreateOrderDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
