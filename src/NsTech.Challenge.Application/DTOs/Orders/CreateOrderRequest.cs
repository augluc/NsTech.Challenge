namespace NsTech.Challenge.Application.DTOs.Orders;

public record CreateOrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderRequest(Guid CustomerId, string Currency, List<CreateOrderItemRequest> Items);