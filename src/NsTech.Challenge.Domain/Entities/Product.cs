namespace NsTech.Challenge.Domain.Entities;

using NsTech.Challenge.Domain.Exceptions;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int AvailableQuantity { get; private set; }

    private Product() { } // Para o EF Core

    public Product(Guid id, string name, decimal unitPrice, int initialStock)
    {
        if (unitPrice <= 0)
            throw new DomainException("Unit price must be greater than zero.");

        if (initialStock < 0)
            throw new DomainException("Initial stock cannot be negative.");

        Id = id;
        Name = name;
        UnitPrice = unitPrice;
        AvailableQuantity = initialStock;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to reserve must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new InsufficientStockException($"Insufficient stock for product {Id}. Requested: {quantity}, Available: {AvailableQuantity}");

        AvailableQuantity -= quantity;
    }

    public void RestoreStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity to restore must be greater than zero.");

        AvailableQuantity += quantity;
    }
}