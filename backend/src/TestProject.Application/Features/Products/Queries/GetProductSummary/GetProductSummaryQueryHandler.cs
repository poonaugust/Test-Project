using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Queries.GetProductSummary;

public sealed class GetProductSummaryQueryHandler(IProductReadStore readStore)
    : IQueryHandler<GetProductSummaryQuery, Result<ProductSummaryDto>>
{
    public async Task<Result<ProductSummaryDto>> Handle(
        GetProductSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var summary = await readStore.GetSummaryAsync(cancellationToken);
        return Result.Success(summary);
    }
}
