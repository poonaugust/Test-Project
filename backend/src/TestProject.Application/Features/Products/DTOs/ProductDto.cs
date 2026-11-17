using TestProject.Domain.Entities;
using TestProject.Domain.Enums;

namespace TestProject.Application.Features.Products.DTOs;

public sealed record ProductDto(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int StockQuantity,
    ProductStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);

public sealed record ProductSummaryDto(
    int TotalProducts,
    int ActiveProducts,
    int LowStockProducts,
    decimal TotalInventoryValue);

public static class ProductMappings
{
    public static ProductDto ToDto(this Product product) =>
        new(
            product.Id,
            product.Sku,
            product.Name,
            product.Description,
            product.Category,
            product.Price,
            product.StockQuantity,
            product.Status,
            product.CreatedAtUtc,
            product.UpdatedAtUtc);
}
