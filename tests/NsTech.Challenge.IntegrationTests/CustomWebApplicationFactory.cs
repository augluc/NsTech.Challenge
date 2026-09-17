namespace NsTech.Challenge.IntegrationTests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NsTech.Challenge.Domain.Entities;
using NsTech.Challenge.Infrastructure.Persistence;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remova os registros anteriores do AppDbContext
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(AppDbContext));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            var optionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (optionsDescriptor != null) services.Remove(optionsDescriptor);

            // Cria um novo ServiceProvider interno exclusivo para o EF InMemory, isolando-o do Npgsql
            var inMemoryServiceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb")
                       .UseInternalServiceProvider(inMemoryServiceProvider);
            });

            // Seeding do banco de dados em memória
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product(Guid.Parse("a1111111-1111-1111-1111-111111111111"), "Mouse Gamer", 150m, 10),
                    new Product(Guid.Parse("b2222222-2222-2222-2222-222222222222"), "Teclado RGB", 300m, 5)
                );
                db.SaveChanges();
            }
        });
    }
}