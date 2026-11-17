using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TestProject.Application;
using TestProject.Application.Common.Exceptions;
using TestProject.Application.Common.Interfaces;
using TestProject.Application.Common.Mediator;
using TestProject.Application.Features.Products.Commands.CreateProduct;

namespace TestProject.UnitTests.Application;

public sealed class MediatorTests
{
    [Fact]
    public async Task Send_InvalidCommand_ShouldTriggerValidationPipelineBehavior()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddScoped<IProductRepository>(_ => Moq.Mock.Of<IProductRepository>());
        services.AddScoped<IUnitOfWork>(_ => Moq.Mock.Of<IUnitOfWork>());

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var invalidCommand = new CreateProductCommand("", "", null, "", -10m, -1);

        var exception = await Should.ThrowAsync<AppValidationException>(
            () => mediator.Send(invalidCommand));

        exception.Errors.ShouldContainKey("Sku");
        exception.Errors.ShouldContainKey("Name");
        exception.Errors.ShouldContainKey("Price");
    }
}
