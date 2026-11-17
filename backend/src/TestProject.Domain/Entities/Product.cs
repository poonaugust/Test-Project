using TestProject.Domain.Common;
using TestProject.Domain.Enums;
using TestProject.Domain.Events;
using TestProject.Domain.Exceptions;

namespace TestProject.Domain.Entities;

public sealed class Product : AuditableEntity
{
    private Product()
    {
    }

    private Product(
        string sku,
        string name,
        string? description,
        string category,
        decimal price,
        int stockQuantity,
        ProductStatus status)
    {
        SetSku(sku);
        SetName(name);
        Description = description?.Trim();
        SetCategory(category);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
        Status = status;

        RaiseDomainEvent(new ProductCreatedEvent(Id, Sku, DateTimeOffset.UtcNow));
    }

    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public ProductStatus Status { get; private set; }

    public static Product Create(
        string sku,
        string name,
        string? description,
        string category,
        decimal price,
        int stockQuantity,
        ProductStatus status = ProductStatus.Active)
    {
        return new Product(sku, name, description, category, price, stockQuantity, status);
    }

    public void Update(
        string sku,
        string name,
        string? description,
        string category,
        decimal price,
        int stockQuantity,
        ProductStatus status)
    {
        SetSku(sku);
        SetName(name);
        Description = description?.Trim();
        SetCategory(category);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
        Status = status;
    }

    private void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainException("Product SKU cannot be empty.");
        }

        Sku = sku.Trim().ToUpperInvariant();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name cannot be empty.");
        }

        Name = name.Trim();
    }

    private void SetCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new DomainException("Product category cannot be empty.");
        }

        Category = category.Trim();
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
        {
            throw new DomainException("Product price cannot be negative.");
        }

        Price = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }

    private void SetStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
        {
            throw new DomainException("Stock quantity cannot be negative.");
        }

        StockQuantity = stockQuantity;
    }
}
