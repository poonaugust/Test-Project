using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Queries.GetProductSummary;

public sealed record GetProductSummaryQuery : IQuery<Result<ProductSummaryDto>>;
