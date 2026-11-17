using TestProject.Application.Common.Mediator;
using TestProject.Application.Common.Models;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;
using TestProject.Domain.Enums;

namespace TestProject.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? Search = null,
    string? Category = null,
    ProductStatus? Status = null,
    string? SortBy = "createdAtUtc",
    bool SortDescending = true,
    int Page = 1,
    int PageSize = 10) : IQuery<Result<PagedResult<ProductDto>>>;
