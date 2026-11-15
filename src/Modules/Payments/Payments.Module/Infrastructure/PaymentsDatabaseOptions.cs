using System.ComponentModel.DataAnnotations;

namespace LedgerFlow.Payments.Infrastructure;

internal sealed class PaymentsDatabaseOptions
{
    public const string SectionName = "Payments:Database";

    [Required(AllowEmptyStrings = false, ErrorMessage = SectionName + ":ConnectionString is required.")]
    public string ConnectionString { get; set; } = string.Empty;
}
