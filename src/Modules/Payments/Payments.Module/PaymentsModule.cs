
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerFlow.Payments;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services)
    {
        return services;
    }

    public static IEndpointRouteBuilder MapPaymentsEndpoints(
        this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var group = app.MapGroup("/payments").WithTags("Payments");

        group.MapGet("/ping", () => Results.Ok(new { module = "payments", status = "alive" }));

        return app;
    }
}
