using LedgerFlow.Orders.Api;
using LedgerFlow.Orders.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LedgerFlow.Orders;

public static class OrdersModule
{
    public static IServiceCollection AddOrdersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<OrdersDatabaseOptions>()
            .Bind(configuration.GetSection(OrdersDatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<OrdersDbContext>((serviceProvider, options) =>
        {
            var database = serviceProvider.GetRequiredService<IOptions<OrdersDatabaseOptions>>().Value;

            options.UseNpgsql(
                database.ConnectionString,
                npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", OrdersDbContext.Schema));
        });

        return services;
    }

    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var group = app.MapGroup("/orders").WithTags("Orders");

        group.MapGet("/ping", () => Results.Ok(new { module = "orders", status = "alive" }));
        OrderEndpoints.Map(group);

        return app;
    }

    public static async Task MigrateOrdersDatabaseAsync(
    this IServiceProvider services,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }
}