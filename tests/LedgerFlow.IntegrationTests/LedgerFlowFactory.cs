using LedgerFlow.Orders;
using LedgerFlow.Payments;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace LedgerFlow.IntegrationTests;

public sealed class LedgerFlowFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("pgvector/pgvector:pg17").Build();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        // Read by Program.cs when the host is built (first use of Services or CreateClient).
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__Postgres",
            _postgres.GetConnectionString());

        await Services.MigrateOrdersDatabaseAsync();
        await Services.MigratePaymentsDatabaseAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}