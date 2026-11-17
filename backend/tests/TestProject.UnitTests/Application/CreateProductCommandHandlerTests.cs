using Moq;
using Shouldly;
using TestProject.Application.Common.Interfaces;
using TestProject.Application.Features.Products.Commands.CreateProduct;
using TestProject.Domain.Common;
using TestProject.Domain.Entities;
using TestProject.Domain.Enums;

namespace TestProject.UnitTests.Application;

public sealed class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task Handle_WhenSkuIsUnique_ShouldCreateProductAndCommit()
    {
        _repositoryMock
            .Setup(r => r.ExistsBySkuAsync("SKU-NEW", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateProductCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        var command = new CreateProductCommand(
            "sku-new",
            "Studio Microphone",
            "USB condenser mic",
            "Audio",
            3490m,
            12,
            ProductStatus.Active);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.Sku.ShouldBe("SKU-NEW");
        result.Value.Name.ShouldBe("Studio Microphone");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSkuAlreadyExists_ShouldReturnConflictFailure()
    {
        _repositoryMock
            .Setup(r => r.ExistsBySkuAsync("SKU-DUP", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateProductCommandHandler(_repositoryMock.Object, _unitOfWorkMock.Object);
        var command = new CreateProductCommand(
            "sku-dup",
            "Duplicate Item",
            null,
            "Audio",
            100m,
            1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.ShouldBeTrue();
        result.Error.Type.ShouldBe(ErrorType.Conflict);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
