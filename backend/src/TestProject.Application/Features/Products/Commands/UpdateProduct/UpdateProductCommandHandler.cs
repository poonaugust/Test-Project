using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure<ProductDto>(
                Error.NotFound("Product.NotFound", $"Product with ID '{request.Id}' was not found."));
        }

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await productRepository.ExistsBySkuAsync(normalizedSku, request.Id, cancellationToken))
        {
            return Result.Failure<ProductDto>(
                Error.Conflict("Product.DuplicateSku", $"A product with SKU '{normalizedSku}' already exists."));
        }

        product.Update(
            request.Sku,
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.Status);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.ToDto());
    }
}
