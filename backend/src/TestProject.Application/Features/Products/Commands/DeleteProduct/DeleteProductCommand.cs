using TestProject.Application.Common.Mediator;
using TestProject.Domain.Common;

namespace TestProject.Application.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand<Result>;
