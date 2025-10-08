using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentWebhookService.Data.Enum
{
    public enum PaymentStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Failed = 3
    }
}
