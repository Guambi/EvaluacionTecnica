using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentWebhookService.Business.Dtos.Payments;
using PaymentWebhookService.Business.Interfaces;
using PaymentWebhookService.Data.Entities;
using PaymentWebhookService.Data.Enum;
using PaymentWebhookService.Infraestructure;

namespace PaymentWebhookService.Business.Implementation
{
    public class PaymentService(PaymentWebhookDbContext context, ILogger<PaymentService> logger) : IPaymentService
    {
        #region Services

        private readonly PaymentWebhookDbContext _context = context;
        private readonly ILogger<PaymentService> _logger = logger;

        #endregion Services

        #region Public Methods

        public async Task<bool> ProcessPaymentNotificationAsync(PaymentNotificationDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingTxn = await _context.Payments
                    .FirstOrDefaultAsync(t => t.TxnId == dto.TxnId);

                if (existingTxn != null)
                {
                    _logger.LogWarning("Duplicate payment notification received: {TxnId}", dto.TxnId);
                    return true;
                }

                // Validate enrollment exists
                var enrollment = await _context.Enrollments.FindAsync(dto.EnrollmentId);
                if (enrollment == null)
                {
                    _logger.LogError("Enrollment not found: {EnrollmentId}", dto.EnrollmentId);
                    return false;
                }

                var paymentTxn = new Payment
                {
                    Id = Guid.NewGuid(),
                    EnrollmentId = dto.EnrollmentId,
                    TxnId = dto.TxnId,
                    Status = dto.Status,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(paymentTxn);

                if (dto.Status == PaymentStatus.Approved)
                {
                    enrollment.Status = EnrollmentStatus.Active;
                    enrollment.UpdatedAt = DateTime.UtcNow;
                }
                else if (dto.Status == PaymentStatus.Rejected || dto.Status == PaymentStatus.Failed)
                {
                    enrollment.Status = EnrollmentStatus.Cancelled;
                    enrollment.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Payment processed: EnrollmentId={EnrollmentId}, TxnId={TxnId}, Status={Status}",
                    dto.EnrollmentId, dto.TxnId, dto.Status);

                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                _logger.LogWarning("Concurrent duplicate payment notification: {TxnId}", dto.TxnId);
                await transaction.RollbackAsync();
                return true; // Idempotency
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment notification: {TxnId}", dto.TxnId);
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion Public Methods

    }
}