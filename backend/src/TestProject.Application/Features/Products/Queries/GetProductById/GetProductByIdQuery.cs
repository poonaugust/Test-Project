using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.DTOs;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<Result<ProductDto>>;
