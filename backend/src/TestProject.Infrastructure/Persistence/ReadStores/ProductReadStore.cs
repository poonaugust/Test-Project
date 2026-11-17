using Microsoft.EntityFrameworkCore;
using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Models;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Enums;

namespace TestProject.Infrastructure.Persistence.ReadStores;

internal sealed class ProductReadStore(ApplicationDbContext dbContext) : IProductReadStore
{
    public async Task<PagedResult<ProductDto>> GetPagedAsync(
        string? search,
        string? category,
        ProductStatus? status,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, pattern) ||
                EF.Functions.ILike(p.Sku, pattern) ||
                EF.Functions.ILike(p.Category, pattern));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var trimmedCategory = category.Trim();
            query = query.Where(p => p.Category == trimmedCategory);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        query = (sortBy?.ToLowerInvariant(), sortDescending) switch
        {
            ("sku", false) => query.OrderBy(p => p.Sku),
            ("sku", true) => query.OrderByDescending(p => p.Sku),
            ("name", false) => query.OrderBy(p => p.Name),
            ("name", true) => query.OrderByDescending(p => p.Name),
            ("category", false) => query.OrderBy(p => p.Category),
            ("category", true) => query.OrderByDescending(p => p.Category),
            ("price", false) => query.OrderBy(p => p.Price),
            ("price", true) => query.OrderByDescending(p => p.Price),
            ("stockquantity", false) => query.OrderBy(p => p.StockQuantity),
            ("stockquantity", true) => query.OrderByDescending(p => p.StockQuantity),
            ("status", false) => query.OrderBy(p => p.Status),
            ("status", true) => query.OrderByDescending(p => p.Status),
            (_, false) => query.OrderBy(p => p.CreatedAtUtc),
            (_, true) => query.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(
                p.Id,
                p.Sku,
                p.Name,
                p.Description,
                p.Category,
                p.Price,
                p.StockQuantity,
                p.Status,
                p.CreatedAtUtc,
                p.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductDto>(items, totalCount, page, pageSize);
    }

    public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(
                p.Id,
                p.Sku,
                p.Name,
                p.Description,
                p.Category,
                p.Price,
                p.StockQuantity,
                p.Status,
                p.CreatedAtUtc,
                p.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<ProductSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var products = dbContext.Products.AsNoTracking();

        var totalProducts = await products.CountAsync(cancellationToken);
        var activeProducts = await products.CountAsync(p => p.Status == ProductStatus.Active, cancellationToken);
        var lowStockProducts = await products.CountAsync(p => p.StockQuantity <= 10, cancellationToken);
        var totalInventoryValue = await products.SumAsync(p => p.Price * p.StockQuantity, cancellationToken);

        return new ProductSummaryDto(
            totalProducts,
            activeProducts,
            lowStockProducts,
            totalInventoryValue);
    }
}
