using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Common.Models;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IProductReadStore readStore)
    : IQueryHandler<GetProductsQuery, Result<PagedResult<ProductDto>>>
{
    public async Task<Result<PagedResult<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var result = await readStore.GetPagedAsync(
            request.Search,
            request.Category,
            request.Status,
            request.SortBy,
            request.SortDescending,
            page,
            pageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
