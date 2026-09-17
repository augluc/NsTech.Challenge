namespace NsTech.Challenge.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using NsTech.Challenge.Application.Common.Interfaces;
using NsTech.Challenge.Domain.Entities;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
        Guid? customerId,
        OrderStatus? status,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.Include(o => o.Items).AsNoTracking().AsQueryable();

        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (from.HasValue)
            query = query.Where(o => o.CreatedAt >= from.Value.ToUniversalTime());

        if (to.HasValue)
            query = query.Where(o => o.CreatedAt <= to.Value.ToUniversalTime());

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}