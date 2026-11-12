using System.Net;
using System.Net.Http.Json;

namespace LedgerFlow.IntegrationTests;

[Collection("api")]
public sealed class PaymentsApiTests(LedgerFlowFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    private sealed record PaymentDto(Guid Id, Guid OrderId, string Status, decimal Amount, string Currency);

    [Fact]
    public async Task CreatingAPaymentReturnsCreatedAndItCanBeFetched()
    {
        var ct = TestContext.Current.CancellationToken;

        var orderId = Guid.NewGuid();
        var paymentResponse = await _client.PostAsJsonAsync(
            "/payments", new { orderId, amount = 125.50m, currency = "eur" }, ct);

        Assert.Equal(HttpStatusCode.Created, paymentResponse.StatusCode);
        Assert.NotNull(paymentResponse.Headers.Location);

        var createdPayment = await paymentResponse.Content.ReadFromJsonAsync<PaymentDto>(ct);

        Assert.NotNull(createdPayment);
        Assert.Equal(orderId, createdPayment.OrderId);
        Assert.Equal("Created", createdPayment.Status);
        Assert.Equal(125.50m, createdPayment.Amount);
        Assert.Equal("EUR", createdPayment.Currency);

        var fetchedPayment = await _client.GetFromJsonAsync<PaymentDto>($"/payments/{createdPayment.Id}", ct);
        Assert.Equal(createdPayment, fetchedPayment);
    }

    [Fact]
    public async Task CreatingAPaymentWithInvalidDataReturnsBadRequest()
    {
        var ct = TestContext.Current.CancellationToken;

        var response = await _client.PostAsJsonAsync(
            "/payments", new { orderId = Guid.NewGuid(), amount = -50m, currency = "us" }, ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FetchingAnUnknownPaymentReturnsNotFound()
    {
        var ct = TestContext.Current.CancellationToken;
        var response = await _client.GetAsync($"/payments/{Guid.NewGuid()}", ct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}
