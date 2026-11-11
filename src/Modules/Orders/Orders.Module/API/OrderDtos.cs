namespace LedgerFlow.Orders.Api;

internal sealed record CreateOrderRequest(decimal TotalAmount, string Currency);

internal sealed record OrderResponse(
    Guid Id,
    string Status,
    decimal TotalAmount,
    string Currency,
    DateTimeOffset CreatedAt);