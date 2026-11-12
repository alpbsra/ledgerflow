using LedgerFlow.Payments.Domain;
using LedgerFlow.Payments.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Payments.API;

internal static class PaymentEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        ArgumentNullException.ThrowIfNull(group);
        group.MapPost("/", CreatePaymentAsync);
        group.MapGet("/{id:guid}", GetPaymentAsync).WithName("GetPayment");
    }

    private static async Task<IResult> CreatePaymentAsync(
        CreatePaymentRequest request,
        PaymentsDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }
        var payment = Payment.Create(request.OrderId, request.Amount, request.Currency);
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken);
        return Results.CreatedAtRoute("GetPayment", new { id = payment.Id }, ToResponse(payment));
    }

    private static async Task<IResult> GetPaymentAsync(
        Guid id,
        PaymentsDbContext db,
        CancellationToken cancellationToken)
    {
        var payment = await db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return payment is null ? Results.NotFound() : Results.Ok(ToResponse(payment));
    }

    private static Dictionary<string, string[]> Validate(CreatePaymentRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        if (request.OrderId == Guid.Empty)
        {
            errors[nameof(request.OrderId)] = ["Order ID cannot be empty."];
        }
        if (request.Amount < 0)
        {
            errors[nameof(request.Amount)] = ["Amount cannot be negative."];
        }
        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            errors[nameof(request.Currency)] = ["Currency must be a valid ISO 4217 currency code."];
        }
        return errors;
    }

    private static PaymentResponse ToResponse(Payment payment) =>
        new(payment.Id, payment.OrderId, payment.Status.ToString(), payment.Amount, payment.Currency, payment.CreatedAt);
}
