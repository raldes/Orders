namespace Orders.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
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
