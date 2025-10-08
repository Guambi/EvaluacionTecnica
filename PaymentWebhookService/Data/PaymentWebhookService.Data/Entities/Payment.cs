using PaymentWebhookService.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentWebhookService.Data.Entities
{
    public class Payment
    {
        public required Guid Id { get; set; }
        public required Guid EnrollmentId { get; set; }
        public string? TxnId { get; set; } = string.Empty; // Unique transaction ID for idempotency
        public required PaymentStatus Status { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
