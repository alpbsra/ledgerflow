namespace LedgerFlow.Payments.Domain
{
    internal sealed class Payment
    {
        private Payment()
        {
        }

        public Guid Id { get; private set; }

        public Guid OrderId { get; private set; }

        public PaymentStatus Status { get; private set; }

        public decimal Amount { get; private set; }

        public string Currency { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; }

        public static Payment Create(Guid orderId, decimal amount, string currency)
        {
            if (orderId == Guid.Empty)
            {
                throw new ArgumentException("Order id is required.", nameof(orderId));
            }
            ArgumentOutOfRangeException.ThrowIfNegative(amount);
            ArgumentException.ThrowIfNullOrWhiteSpace(currency);

            if (currency.Length != 3)
            {
                throw new ArgumentException("Currency must be a 3-letter ISO code.", nameof(currency));
            }

            return new Payment
            {
                Id = Guid.CreateVersion7(),
                OrderId = orderId,
                Status = PaymentStatus.Created,
                Amount = amount,
                Currency = currency.ToUpperInvariant(),
                CreatedAt = DateTime.UtcNow
            };
        }


    }
}
