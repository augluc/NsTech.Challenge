namespace NsTech.Challenge.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NsTech.Challenge.Domain.Entities;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(o => o.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>() // Salva como STRING no banco (facilita leitura e audit)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        // Mapeamento do backing field _items
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice para otimizar as consultas paginadas e filtradas por cliente/status/data
        builder.HasIndex(o => new { o.CustomerId, o.Status, o.CreatedAt });
    }
}