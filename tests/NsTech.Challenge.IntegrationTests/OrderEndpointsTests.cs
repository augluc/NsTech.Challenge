namespace NsTech.Challenge.IntegrationTests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using NsTech.Challenge.Application.DTOs.Orders;
using Xunit;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrderEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AuthenticateAsync()
    {
        var loginResponse = await _client.PostAsJsonAsync("/auth/token", new
        {
            Username = "nstech",
            Password = "admin123"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var token = result!["token"];

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateOrder_WithValidPayload_ShouldReturnCreated()
    {
        // Arrange
        await AuthenticateAsync();

        var payload = new CreateOrderRequest(
            CustomerId: Guid.NewGuid(),
            Currency: "BRL",
            Items: new List<CreateOrderItemRequest>
            {
                new(Guid.Parse("a1111111-1111-1111-1111-111111111111"), 2)
            }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/orders", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>();
        order.Should().NotBeNull();
        order!.Total.Should().Be(300m); // 150 * 2
        order.Status.Should().Be("Placed");
    }

    [Fact]
    public async Task ConfirmAndCancelOrder_ShouldBeIdempotent()
    {
        // Arrange
        await AuthenticateAsync();

        var createPayload = new CreateOrderRequest(
            CustomerId: Guid.NewGuid(),
            Currency: "BRL",
            Items: new List<CreateOrderItemRequest>
            {
                new(Guid.Parse("b2222222-2222-2222-2222-222222222222"), 1)
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/orders", createPayload);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderResponse>();

        // Act 1: Primeira Confirmação
        var confirm1 = await _client.PostAsync($"/orders/{createdOrder!.Id}/confirm", null);
        confirm1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act 2: Segunda Confirmação (Idempotência)
        var confirm2 = await _client.PostAsync($"/orders/{createdOrder.Id}/confirm", null);
        confirm2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act 3: Cancelamento
        var cancel = await _client.PostAsync($"/orders/{createdOrder.Id}/cancel", null);
        cancel.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateOrder_WithoutToken_ShouldReturnUnauthorized()
    {
        // Arrange (sem token de autenticação)
        _client.DefaultRequestHeaders.Authorization = null;

        var payload = new CreateOrderRequest(
            CustomerId: Guid.NewGuid(),
            Currency: "BRL",
            Items: new List<CreateOrderItemRequest>
            {
                new(Guid.Parse("a1111111-1111-1111-1111-111111111111"), 1)
            }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/orders", payload);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}