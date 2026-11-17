using TestProject.Domain.Entities;

namespace TestProject.Application.Common.Interfaces;

/// <summary>
/// Write-side repository contract (CQRS Command side).
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Remove(Product product);
}
