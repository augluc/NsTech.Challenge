namespace NsTech.Challenge.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NsTech.Challenge.Domain.Entities;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.AvailableQuantity)
            .IsRequired();

        // Controle de Concorrência Otimista (Optimistic Concurrency)
        // Impede que duas requisições simultâneas comprem o último item do estoque incorretamente
        builder.Property<uint>("Version")
            .IsRowVersion();
    }
}