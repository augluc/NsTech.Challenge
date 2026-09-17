namespace NsTech.Challenge.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) {; }
}

public class InvalidOrderStateException : DomainException
{
    public InvalidOrderStateException(string message) : base(message) {; }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string message) : base(message) {; }
}