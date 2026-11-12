namespace LedgerFlow.Payments.API;

internal sealed record CreatePaymentRequest(Guid OrderId, decimal Amount, string Currency);

internal sealed record PaymentResponse(
    Guid Id,
    Guid OrderId,
    string Status,
    decimal Amount,
    string Currency,
    DateTimeOffset CreatedAt);