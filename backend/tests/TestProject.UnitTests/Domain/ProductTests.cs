using Shouldly;
using TestProject.Domain.Entities;
using TestProject.Domain.Enums;
using TestProject.Domain.Events;
using TestProject.Domain.Exceptions;

namespace TestProject.UnitTests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldNormalizeSkuAndRaiseDomainEvent()
    {
        var product = Product.Create(
            "  sku-100 ",
            "  USB-C Hub ",
            "Compact hub",
            "Accessories",
            1299.456m,
            15,
            ProductStatus.Active);

        product.Sku.ShouldBe("SKU-100");
        product.Name.ShouldBe("USB-C Hub");
        product.Price.ShouldBe(1299.46m);
        product.StockQuantity.ShouldBe(15);
        product.DomainEvents.Count.ShouldBe(1);
        product.DomainEvents.First().ShouldBeOfType<ProductCreatedEvent>();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowDomainException()
    {
        var act = () => Product.Create("SKU-1", "Item", null, "General", -1m, 1);

        act.ShouldThrow<DomainException>();
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowDomainException()
    {
        var act = () => Product.Create("SKU-1", "Item", null, "General", 10m, -5);

        act.ShouldThrow<DomainException>();
    }
}
