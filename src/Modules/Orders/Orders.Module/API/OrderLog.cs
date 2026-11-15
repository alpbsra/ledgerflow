using LedgerFlow.Orders.Domain;
using Microsoft.Extensions.Logging;

namespace LedgerFlow.Orders.Api;

internal static partial class OrderLog
{
    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Order created {OrderId} and status {Status}")]
    public static partial void OrderCreated(ILogger logger, Guid orderId, OrderStatus status);
}
