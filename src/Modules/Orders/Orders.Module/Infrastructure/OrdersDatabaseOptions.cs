using System.ComponentModel.DataAnnotations;

namespace LedgerFlow.Orders.Infrastructure;

internal sealed class OrdersDatabaseOptions
{
    public const string SectionName = "Orders:Database";

    [Required(AllowEmptyStrings = false, ErrorMessage = SectionName + ":ConnectionString is required.")]
    public string ConnectionString { get; set; } = string.Empty;
}