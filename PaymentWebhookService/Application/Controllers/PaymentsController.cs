using Microsoft.AspNetCore.Mvc;
using PaymentWebhookService.Api.Middlewares;
using PaymentWebhookService.Business.Dtos.Payments;
using PaymentWebhookService.Business.Interfaces;

namespace PaymentWebhookService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger) : ControllerBase
    {
        #region Services

        private readonly IPaymentService _paymentService = paymentService;
        private readonly ILogger<PaymentsController> _logger = logger;

        #endregion Services

        #region Webhooks

        /// <summary>
        /// Webhook endpoint for payment notifications (idempotent)
        /// </summary>
        /// <remarks>
        /// Requires X-API-Key header for authentication.
        /// Supports idempotent retries using txnId.
        /// </remarks>
        [HttpPost("notify")]
        [ServiceFilter(typeof(WebhookApiKeyFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> NotifyPayment([FromBody] PaymentNotificationDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid payment notification received");
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                "Payment notification received: EnrollmentId={EnrollmentId}, TxnId={TxnId}, Status={Status}",
                dto.EnrollmentId, dto.TxnId, dto.Status);

            try
            {
                var result = await _paymentService.ProcessPaymentNotificationAsync(dto);

                if (!result)
                {
                    _logger.LogError("Failed to process payment notification for enrollment {EnrollmentId}",
                        dto.EnrollmentId);
                    return BadRequest(new { message = "Failed to process payment notification" });
                }

                return Ok(new
                {
                    message = "Payment notification processed successfully",
                    txnId = dto.TxnId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment notification: {TxnId}", dto.TxnId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        #endregion Webhooks
    }
}