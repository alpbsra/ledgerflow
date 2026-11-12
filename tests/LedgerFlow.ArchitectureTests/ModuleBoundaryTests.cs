using System.Reflection;
using LedgerFlow.Orders;
using LedgerFlow.Payments;

namespace LedgerFlow.ArchitectureTests;

public sealed class ModuleBoundaryTests
{
    private static readonly Assembly[] ModuleAssemblies =
    [
        typeof(OrdersModule).Assembly,
        typeof(PaymentsModule).Assembly
    ];

    private static readonly string[] SupportAssemblyNames =
    [
        "LedgerFlow.Orders.Contracts",
        "LedgerFlow.Payments.Contracts",
        "LedgerFlow.SharedKernel"
    ];

    [Fact]
    public void ModulesDoNotReferenceEachOthersInternals()
    {
        foreach (var assembly in ModuleAssemblies)
        {
            var own = assembly.GetName().Name;
            var offending = ModuleReferences(assembly)
                .Where(name => name != own)
                .ToList();

            Assert.True(
                offending.Count == 0,
                $"{own} must use other modules only through their Contracts, but references: {string.Join(", ", offending)}");
        }
    }

    [Fact]
    public void ContractsAndSharedKernelDoNotReferenceAnyModule()
    {
        foreach (var name in SupportAssemblyNames)
        {
            var offending = ModuleReferences(Assembly.Load(name)).ToList();

            Assert.True(
                offending.Count == 0,
                $"{name} must not depend on module internals, but references: {string.Join(", ", offending)}");
        }
    }

    private static IEnumerable<string> ModuleReferences(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .OfType<string>()
            .Where(name => name.StartsWith("LedgerFlow.", StringComparison.Ordinal)
                && name.EndsWith(".Module", StringComparison.Ordinal));
}