using LedgerFlow.Payments.Domain;
using Microsoft.EntityFrameworkCore;

namespace LedgerFlow.Payments.Infrastructure
{
    internal sealed class PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : DbContext(options)
    {
        public const string Schema = "payments";

        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedNever();
                entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
                entity.Property(p => p.Amount).HasPrecision(18, 2);
                entity.Property(p => p.Currency).HasMaxLength(3);
                entity.HasIndex(p => p.OrderId);
            });
        }
    }
}
