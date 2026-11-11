using LedgerFlow.Orders.Domain;
using LedgerFlow.Orders.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Orders.Api;

internal static class OrderEndpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        ArgumentNullException.ThrowIfNull(group);

        group.MapPost("/", CreateOrderAsync);
        group.MapGet("/{id:guid}", GetOrderAsync).WithName("GetOrder");
    }

    private static async Task<IResult> CreateOrderAsync(
        CreateOrderRequest request,
        OrdersDbContext db,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
        {
            return Results.ValidationProblem(errors);
        }

        var order = Order.Create(request.TotalAmount, request.Currency);

        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellationToken);

        return Results.CreatedAtRoute("GetOrder", new { id = order.Id }, ToResponse(order));
    }

    private static async Task<IResult> GetOrderAsync(
        Guid id,
        OrdersDbContext db,
        CancellationToken cancellationToken)
    {
        var order = await db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return order is null ? Results.NotFound() : Results.Ok(ToResponse(order));
    }

    private static Dictionary<string, string[]> Validate(CreateOrderRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.TotalAmount < 0)
        {
            errors[nameof(request.TotalAmount)] = ["Total amount cannot be negative."];
        }

        if (string.IsNullOrWhiteSpace(request.Currency) || request.Currency.Length != 3)
        {
            errors[nameof(request.Currency)] = ["Currency must be a 3-letter ISO code."];
        }

        return errors;
    }

    private static OrderResponse ToResponse(Order order) =>
        new(order.Id, order.Status.ToString(), order.TotalAmount, order.Currency, order.CreatedAt);
}