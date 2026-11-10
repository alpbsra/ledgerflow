namespace LedgerFlow.Orders.Domain;

internal sealed class Order
{
    // Used by EF Core only.
    private Order()
    {
    }

    public Guid Id { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal TotalAmount { get; private set; }

    public string Currency { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static Order Create(decimal totalAmount, string currency)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalAmount);
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);

        if (currency.Length != 3)
        {
            throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));
        }

        return new Order
        {
            Id = Guid.CreateVersion7(),
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            Currency = currency.ToUpperInvariant(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}