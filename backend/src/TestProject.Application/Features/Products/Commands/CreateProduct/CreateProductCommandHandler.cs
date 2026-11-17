using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;
using TestProject.Domain.Entities;

namespace TestProject.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await productRepository.ExistsBySkuAsync(normalizedSku, cancellationToken: cancellationToken))
        {
            return Result.Failure<ProductDto>(
                Error.Conflict("Product.DuplicateSku", $"A product with SKU '{normalizedSku}' already exists."));
        }

        var product = Product.Create(
            request.Sku,
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.Status);

        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.ToDto());
    }
}
