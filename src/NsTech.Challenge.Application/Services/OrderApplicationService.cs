namespace NsTech.Challenge.Application.Services;

using NsTech.Challenge.Application.Common.Interfaces;
using NsTech.Challenge.Application.DTOs.Orders;
using NsTech.Challenge.Domain.Entities;
using NsTech.Challenge.Domain.Exceptions;

public interface IOrderApplicationService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    Task<bool> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken);
    Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<OrderResponse>> GetPagedAsync(
        Guid? customerId, OrderStatus? status, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken cancellationToken);
}

public class OrderApplicationService : IOrderApplicationService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderApplicationService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var productIds = request.Items.Select(i => i.ProductId).Distinct();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        if (products.Count != productIds.Count())
            throw new DomainException("One or more products were not found.");

        var order = new Order(request.CustomerId, request.Currency);

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);

            // Valida e reserva estoque
            product.ReserveStock(item.Quantity);
            order.AddItem(product.Id, product.UnitPrice, item.Quantity);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _productRepository.UpdateRangeAsync(products, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(order);
    }

    public async Task<bool> ConfirmOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{orderId}' not found.");

        var changed = order.Confirm();
        if (changed)
        {
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return changed;
    }

    public async Task<bool> CancelOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order '{orderId}' not found.");

        var previousStatus = order.Status;
        var changed = order.Cancel();

        if (changed)
        {
            // Se o pedido estava em Placed/Confirmed, devolve o estoque dos produtos
            var productIds = order.Items.Select(i => i.ProductId).Distinct();
            var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

            foreach (var item in order.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                product?.RestoreStock(item.Quantity);
            }

            await _productRepository.UpdateRangeAsync(products, cancellationToken);
            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return changed;
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<PagedResult<OrderResponse>> GetPagedAsync(
        Guid? customerId, OrderStatus? status, DateTime? from, DateTime? to, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _orderRepository.GetPagedAsync(customerId, status, from, to, page, pageSize, cancellationToken);
        var responses = items.Select(MapToResponse);
        return new PagedResult<OrderResponse>(responses, totalCount, page, pageSize);
    }

    private static OrderResponse MapToResponse(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.Currency,
            order.Total,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemResponse(i.ProductId, i.UnitPrice, i.Quantity, i.CalculateTotal())).ToList()
        );
}