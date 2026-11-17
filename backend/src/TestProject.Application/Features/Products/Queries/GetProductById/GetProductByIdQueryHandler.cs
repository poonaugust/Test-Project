using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductReadStore readStore)
    : IQueryHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await readStore.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductDto>(
                Error.NotFound("Product.NotFound", $"Product with ID '{request.Id}' was not found."));
        }

        return Result.Success(product);
    }
}
