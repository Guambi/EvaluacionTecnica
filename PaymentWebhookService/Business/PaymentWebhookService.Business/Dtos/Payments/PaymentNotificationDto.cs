using PaymentWebhookService.Data.Enum;

namespace PaymentWebhookService.Business.Dtos.Payments
{
    public record PaymentNotificationDto(Guid EnrollmentId, PaymentStatus Status, string TxnId);
}