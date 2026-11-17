using Microsoft.EntityFrameworkCore;
using TestProject.Application.Common.Interfaces;
using TestProject.Domain.Entities;

namespace TestProject.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default) =>
        dbContext.Products.AnyAsync(
            p => p.Sku == sku && (!excludeId.HasValue || p.Id != excludeId.Value),
            cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await dbContext.Products.AddAsync(product, cancellationToken);

    public void Remove(Product product) => dbContext.Products.Remove(product);
}
