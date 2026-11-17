using Microsoft.AspNetCore.Mvc;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Common.Models;
using TestProject.Application.Features.Products.Commands.CreateProduct;
using TestProject.Application.Features.Products.Commands.DeleteProduct;
using TestProject.Application.Features.Products.Commands.UpdateProduct;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Application.Features.Products.Queries.GetProductById;
using TestProject.Application.Features.Products.Queries.GetProducts;
using TestProject.Application.Features.Products.Queries.GetProductSummary;
using TestProject.Domain.Enums;

namespace TestProject.WebApi.Controllers;

[Route("api/v1/products")]
public sealed class ProductsController(IMediator mediator) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] ProductStatus? status,
        [FromQuery] string? sortBy = "createdAtUtc",
        [FromQuery] bool sortDescending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(search, category, status, sortBy, sortDescending, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ProductSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductSummaryQuery(), cancellationToken);
        return ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Sku,
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.Status);

        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Sku,
            request.Name,
            request.Description,
            request.Category,
            request.Price,
            request.StockQuantity,
            request.Status);

        var result = await mediator.Send(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return ToActionResult(result);
    }
}

public sealed record CreateProductRequest(
    string Sku,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int StockQuantity,
    ProductStatus Status = ProductStatus.Active);

public sealed record UpdateProductRequest(
    string Sku,
    string Name,
    string? Description,
    string Category,
    decimal Price,
    int StockQuantity,
    ProductStatus Status);
