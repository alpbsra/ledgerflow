
using LedgerFlow.Payments.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LedgerFlow.Payments.API;

namespace LedgerFlow.Payments;

public static class PaymentsModule
{
    public static IServiceCollection AddPaymentsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");

        services.AddDbContext<PaymentsDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", PaymentsDbContext.Schema)));
        return services;
    }

    public static IEndpointRouteBuilder MapPaymentsEndpoints(
        this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var group = app.MapGroup("/payments").WithTags("Payments");

        group.MapGet("/ping", () => Results.Ok(new { module = "payments", status = "alive" }));
        PaymentEndpoints.Map(group);

        return app;
    }

    public static async Task MigratePaymentsDatabaseAsync(
    this IServiceProvider services,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }
}
