using PaymentWebhookService.Business.Dtos.Payments;

namespace PaymentWebhookService.Business.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentNotificationAsync(PaymentNotificationDto dto);
    }
}