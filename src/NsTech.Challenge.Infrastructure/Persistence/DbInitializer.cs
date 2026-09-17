namespace NsTech.Challenge.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using NsTech.Challenge.Domain.Entities;

public static class DbInitializer
{
    public static async Task MigrateAndSeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Products.AnyAsync())
        {
            var initialProducts = new List<Product>
            {
                new(Guid.Parse("a1111111-1111-1111-1111-111111111111"), "Mouse Gamer Logtech", 150.00m, initialStock: 50),
                new(Guid.Parse("b2222222-2222-2222-2222-222222222222"), "Teclado Mecânico RGB", 350.00m, initialStock: 20),
                new(Guid.Parse("c3333333-3333-3333-3333-333333333333"), "Monitor Ultrawide 29", 1200.00m, initialStock: 5)
            };

            await context.Products.AddRangeAsync(initialProducts);
            await context.SaveChangesAsync();
        }
    }
}