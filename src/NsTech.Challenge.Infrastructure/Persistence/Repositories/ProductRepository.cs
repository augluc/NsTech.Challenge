namespace NsTech.Challenge.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using NsTech.Challenge.Application.Common.Interfaces;
using NsTech.Challenge.Domain.Entities;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default)
    {
        _context.Products.UpdateRange(products);
        return Task.CompletedTask;
    }
}