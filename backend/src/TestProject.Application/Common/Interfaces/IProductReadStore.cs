using TestProject.Application.Common.Models;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Enums;

namespace TestProject.Application.Common.Interfaces;

/// <summary>
/// Read-side projection contract (CQRS Query side), bypassing domain tracking overhead.
/// </summary>
public interface IProductReadStore
{
    Task<PagedResult<ProductDto>> GetPagedAsync(
        string? search,
        string? category,
        ProductStatus? status,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProductSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
