using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;
using TestProject.Domain.Enums;

namespace TestProject.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int StockQuantity,
    ProductStatus Status) : ICommand<Result<ProductDto>>;
