using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestProject.Domain.Entities;
using TestProject.Domain.Enums;

namespace TestProject.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        bool autoMigrate,
        CancellationToken cancellationToken = default)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DbInitializer");

        if (autoMigrate)
        {
            logger.LogInformation("Applying pending EF Core migrations...");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }

        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        logger.LogInformation("Seeding initial product catalog data...");
        var seedProducts = new[]
        {
            Product.Create(
                "TECH-001",
                "Mechanical Keyboard Pro",
                "Hot-swappable 75% aluminum mechanical keyboard with tactile switches.",
                "Peripherals",
                4290.00m,
                25,
                ProductStatus.Active),
            Product.Create(
                "TECH-002",
                "4K USB-C Monitor 27\"",
                "IPS UHD display with 99% sRGB coverage and 90W Power Delivery.",
                "Monitors",
                14900.00m,
                8,
                ProductStatus.Active),
            Product.Create(
                "TECH-003",
                "Ergonomic Wireless Mouse",
                "Multi-device Bluetooth and 2.4GHz mouse with precision optical sensor.",
                "Peripherals",
                2490.00m,
                42,
                ProductStatus.Active),
            Product.Create(
                "TECH-004",
                "Thunderbolt 4 Docking Station",
                "12-in-1 desktop hub supporting dual 4K displays and 2.5Gb Ethernet.",
                "Accessories",
                6850.00m,
                5,
                ProductStatus.Active),
            Product.Create(
                "TECH-005",
                "Noise-Cancelling Studio Headset",
                "Upcoming wireless ANC headset currently finalizing certification.",
                "Audio",
                7900.00m,
                0,
                ProductStatus.Draft)
        };

        await dbContext.Products.AddRangeAsync(seedProducts, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} initial products.", seedProducts.Length);
    }
}
