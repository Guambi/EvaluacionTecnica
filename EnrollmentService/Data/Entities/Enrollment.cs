using EnrollmentService.Data.Enum;

namespace EnrollmentService.Data.Entities
{
    public class Enrollment
    {
        public required Guid Id { get; set; }
        public required string StudentId { get; set; } = string.Empty;
        public required EnrollmentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
