namespace NsTech.Challenge.Application.DTOs.Orders;

public record OrderItemResponse(Guid ProductId, decimal UnitPrice, int Quantity, decimal Total);

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    string Currency,
    decimal Total,
    DateTime CreatedAt,
    List<OrderItemResponse> Items);

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);