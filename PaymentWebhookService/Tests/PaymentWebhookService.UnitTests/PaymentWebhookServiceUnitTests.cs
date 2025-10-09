using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentWebhookService.Business.Dtos.Payments;
using PaymentWebhookService.Business.Implementation;
using PaymentWebhookService.Data.Entities;
using PaymentWebhookService.Data.Enum;
using PaymentWebhookService.Infraestructure;

namespace PaymentWebhookService.UnitTests
{
    public class PaymentWebhookServiceUnitTests : IDisposable
    {
        private readonly PaymentWebhookDbContext _context;
        private readonly Mock<ILogger<PaymentService>> _loggerMock;
        private readonly PaymentService _service;

        public PaymentWebhookServiceUnitTests()
        {
            var options = new DbContextOptionsBuilder<PaymentWebhookDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new PaymentWebhookDbContext(options);
            _loggerMock = new Mock<ILogger<PaymentService>>();
            _service = new PaymentService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task ProcessPaymentNotificationAsync_ShouldUpdateEnrollment_WhenApproved()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU005",
                Status = EnrollmentStatus.Pending,
                Amount = 3000.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var notification = new PaymentNotificationDto(enrollment.Id, PaymentStatus.Approved, "TXN-12345");

            // Act
            var result = await _service.ProcessPaymentNotificationAsync(notification);

            // Assert
            Assert.True(result);
            var updated = await _context.Enrollments.FindAsync(enrollment.Id);
            Assert.Equal(EnrollmentStatus.Active, updated!.Status);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public async Task ProcessPaymentNotificationAsync_ShouldCancelEnrollment_WhenRejected()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU006",
                Status = EnrollmentStatus.Pending,
                Amount = 2500.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var notification = new PaymentNotificationDto(enrollment.Id, PaymentStatus.Rejected, "TXN-FAILED");

            // Act
            var result = await _service.ProcessPaymentNotificationAsync(notification);

            // Assert
            Assert.True(result);
            var updated = await _context.Enrollments.FindAsync(enrollment.Id);
            Assert.Equal(EnrollmentStatus.Cancelled, updated!.Status);
        }

        [Fact]
        public async Task ProcessPaymentNotificationAsync_ShouldCancelEnrollment_WhenFailed()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU007",
                Status = EnrollmentStatus.Pending,
                Amount = 1800.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var notification = new PaymentNotificationDto(enrollment.Id, PaymentStatus.Failed, "TXN-FAILED");            

            // Act
            var result = await _service.ProcessPaymentNotificationAsync(notification);

            // Assert
            Assert.True(result);
            var updated = await _context.Enrollments.FindAsync(enrollment.Id);
            Assert.Equal(EnrollmentStatus.Cancelled, updated!.Status);
        }

        [Fact]
        public async Task ProcessPaymentNotificationAsync_ShouldBeIdempotent()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU008",
                Status = EnrollmentStatus.Pending,
                Amount = 2500.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var notification = new PaymentNotificationDto(enrollment.Id, PaymentStatus.Approved, "TXN-DUPLICATE-123");

            // Act - Process twice with same txnId
            var result1 = await _service.ProcessPaymentNotificationAsync(notification);
            var result2 = await _service.ProcessPaymentNotificationAsync(notification);

            // Assert
            Assert.True(result1);
            Assert.True(result2); // Should succeed due to idempotency

            var transactions = await _context.Payments
                .Where(t => t.TxnId == "TXN-DUPLICATE-123")
                .ToListAsync();

            Assert.Single(transactions); // Only one transaction should exist
        }

        [Fact]
        public async Task ProcessPaymentNotificationAsync_ShouldReturnFalse_WhenEnrollmentNotExists()
        {
            // Arrange
            var notification = new PaymentNotificationDto(Guid.NewGuid(), PaymentStatus.Approved, "TXN-NOTFOUND");

            // Act
            var result = await _service.ProcessPaymentNotificationAsync(notification);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessPaymentNotificationAsync_ShouldCreatePaymentTransaction()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU009",
                Status = EnrollmentStatus.Pending,
                Amount = 4000.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var notification = new PaymentNotificationDto(enrollment.Id, PaymentStatus.Approved, "TXN-RECORD-123");

            // Act
            await _service.ProcessPaymentNotificationAsync(notification);

            // Assert
            var transaction = await _context.Payments
                .FirstOrDefaultAsync(t => t.TxnId == "TXN-RECORD-123");

            Assert.NotNull(transaction);
            Assert.Equal(enrollment.Id, transaction.EnrollmentId);
            Assert.Equal(PaymentStatus.Approved, transaction.Status);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}