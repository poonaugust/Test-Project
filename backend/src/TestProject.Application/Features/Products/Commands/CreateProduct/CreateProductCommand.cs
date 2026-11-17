using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;
using TestProject.Domain.Enums;

namespace TestProject.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Sku,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int StockQuantity,
    ProductStatus Status = ProductStatus.Active) : ICommand<Result<ProductDto>>;
