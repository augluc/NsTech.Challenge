namespace NsTech.Challenge.UnitTests.Domain;

using FluentAssertions;
using NsTech.Challenge.Domain.Entities;
using NsTech.Challenge.Domain.Exceptions;

public class OrderTests
{
    [Fact]
    public void CreateOrder_WithValidData_ShouldInitializeAsPlacedAndCalculateTotal()
    {
        // Arrange & Act
        var order = new Order(Guid.NewGuid(), "BRL");
        order.AddItem(Guid.NewGuid(), unitPrice: 50.0m, quantity: 2);
        order.AddItem(Guid.NewGuid(), unitPrice: 30.0m, quantity: 1);

        // Assert
        order.Status.Should().Be(OrderStatus.Placed);
        order.Total.Should().Be(130.0m);
        order.Items.Should().HaveCount(2);
    }

    [Fact]
    public void ConfirmOrder_WhenStatusIsPlaced_ShouldTransitionToConfirmed()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), "BRL");

        // Act
        var changed = order.Confirm();

        // Assert
        changed.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void ConfirmOrder_WhenAlreadyConfirmed_ShouldBeIdempotentAndReturnFalse()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), "BRL");
        order.Confirm();

        // Act
        var result = order.Confirm();

        // Assert
        result.Should().BeFalse();
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void CancelOrder_WhenPlacedOrConfirmed_ShouldTransitionToCanceled()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), "BRL");

        // Act
        var result = order.Cancel();

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Canceled);
    }

    [Fact]
    public void ReserveStock_WhenQuantityExceedsAvailable_ShouldThrowInsufficientStockException()
    {
        // Arrange
        var product = new Product(Guid.NewGuid(), "Item Teste", 100m, initialStock: 5);

        // Act & Assert
        var act = () => product.ReserveStock(10);
        act.Should().Throw<InsufficientStockException>();
    }
}