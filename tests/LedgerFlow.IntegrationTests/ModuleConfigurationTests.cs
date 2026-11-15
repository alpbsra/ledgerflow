using LedgerFlow.Orders;
using LedgerFlow.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LedgerFlow.IntegrationTests;

public class ModuleConfigurationTests
{
    [Fact]
    public void OrdersWithoutConnectionStringFailsStartupValidation()
    {
        var configuration = new ConfigurationBuilder().Build();

        using var provider = new ServiceCollection()
            .AddOrdersModule(configuration)
            .BuildServiceProvider();

        var validator = provider.GetRequiredService<IStartupValidator>();

        var exception = Assert.Throws<OptionsValidationException>(validator.Validate);
        Assert.Contains("Orders:Database:ConnectionString", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void OrdersWithConnectionStringPassesStartupValidation()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Orders:Database:ConnectionString"] = "Host=localhost;Database=x"
            })
            .Build();

        using var provider = new ServiceCollection()
            .AddOrdersModule(configuration)
            .BuildServiceProvider();

        var validator = provider.GetRequiredService<IStartupValidator>();
        Assert.Null(Record.Exception(() => validator.Validate()));
    }

    [Fact]
    public void PaymentsWithoutConnectionStringFailsStartupValidation()
    {
        var configuration = new ConfigurationBuilder().Build();

        using var provider = new ServiceCollection()
            .AddPaymentsModule(configuration)
            .BuildServiceProvider();

        var validator = provider.GetRequiredService<IStartupValidator>();

        var exception = Assert.Throws<OptionsValidationException>(validator.Validate);
        Assert.Contains("Payments:Database:ConnectionString", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void PaymentsWithConnectionStringPassesStartupValidation()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Payments:Database:ConnectionString"] = "Host=localhost;Database=x"
            })
            .Build();

        using var provider = new ServiceCollection()
            .AddPaymentsModule(configuration)
            .BuildServiceProvider();

        var validator = provider.GetRequiredService<IStartupValidator>();
        Assert.Null(Record.Exception(() => validator.Validate()));
    }
}
