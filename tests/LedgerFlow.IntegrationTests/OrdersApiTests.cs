using System.Net;
using System.Net.Http.Json;

namespace LedgerFlow.IntegrationTests;

[Collection("api")]
public sealed class OrdersApiTests(LedgerFlowFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private sealed record OrderDto(Guid Id, string Status, decimal TotalAmount, string Currency);

    [Fact]
    public async Task CreatingAnOrderReturnsCreatedAndItCanBeFetched()
    {
        var ct = TestContext.Current.CancellationToken;

        var response = await _client.PostAsJsonAsync(
            "/orders", new { totalAmount = 125.50m, currency = "eur" }, ct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<OrderDto>(ct);
        Assert.NotNull(created);
        Assert.Equal("Pending", created.Status);
        Assert.Equal("EUR", created.Currency);
        Assert.Equal(125.50m, created.TotalAmount);

        var fetched = await _client.GetFromJsonAsync<OrderDto>($"/orders/{created.Id}", ct);
        Assert.Equal(created, fetched);
    }

    [Fact]
    public async Task CreatingAnOrderWithInvalidDataReturnsBadRequest()
    {
        var ct = TestContext.Current.CancellationToken;

        var response = await _client.PostAsJsonAsync(
            "/orders", new { totalAmount = -5m, currency = "EU" }, ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FetchingAnUnknownOrderReturnsNotFound()
    {
        var ct = TestContext.Current.CancellationToken;

        var response = await _client.GetAsync($"/orders/{Guid.NewGuid()}", ct);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}