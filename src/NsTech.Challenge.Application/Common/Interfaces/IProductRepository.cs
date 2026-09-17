namespace NsTech.Challenge.Application.Common.Interfaces;

using NsTech.Challenge.Domain.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken = default);
}