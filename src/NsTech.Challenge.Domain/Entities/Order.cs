namespace NsTech.Challenge.Domain.Entities;

using NsTech.Challenge.Domain.Exceptions;

public class Order
{
    private readonly List<OrderItem> _items = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public Order(Guid customerId, string currency)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId is required.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Currency = currency.ToUpperInvariant();
        Status = OrderStatus.Placed;
        CreatedAt = DateTime.UtcNow;
        Total = 0m;
    }

    public void AddItem(Guid productId, decimal unitPrice, int quantity)
    {
        if (Status != OrderStatus.Placed)
            throw new InvalidOrderStateException("Items can only be added to Placed orders.");

        var item = new OrderItem(productId, unitPrice, quantity);
        _items.Add(item);
        RecalculateTotal();
    }

    public bool Confirm()
    {
        if (Status == OrderStatus.Confirmed)
            return false;

        if (Status != OrderStatus.Placed)
            throw new InvalidOrderStateException($"Cannot confirm order in status '{Status}'.");

        Status = OrderStatus.Confirmed;
        return true;
    }

    public bool Cancel()
    {
        if (Status == OrderStatus.Canceled)
            return false;

        if (Status != OrderStatus.Placed && Status != OrderStatus.Confirmed)
            throw new InvalidOrderStateException($"Cannot cancel order in status '{Status}'.");

        Status = OrderStatus.Canceled;
        return true;
    }

    private void RecalculateTotal()
    {
        Total = _items.Sum(item => item.CalculateTotal());
    }
}