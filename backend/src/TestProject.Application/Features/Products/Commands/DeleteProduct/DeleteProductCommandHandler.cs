using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure(
                Error.NotFound("Product.NotFound", $"Product with ID '{request.Id}' was not found."));
        }

        productRepository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
