using LedgerFlow.Payments.Domain;
using Microsoft.Extensions.Logging;

namespace LedgerFlow.Payments.API;

internal static partial class PaymentLog
{
    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "Payment created {PaymentId} and status {Status}")]
    public static partial void PaymentCreated(ILogger logger, Guid paymentId, PaymentStatus status);
}
