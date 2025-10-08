using Microsoft.EntityFrameworkCore;
using PaymentWebhookService.Data.Entities;

namespace PaymentWebhookService.Infraestructure;

public class PaymentWebhookDbContext(DbContextOptions<PaymentWebhookDbContext> options) : DbContext(options)
{
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StudentId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasIndex(e => e.StudentId);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TxnId).IsRequired().HasMaxLength(200);

            // Unique constraint for idempotency
            entity.HasIndex(e => e.TxnId).IsUnique();
        });
    }
}