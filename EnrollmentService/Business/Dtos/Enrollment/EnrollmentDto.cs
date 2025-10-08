using EnrollmentService.Data.Enum;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentDto
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public EnrollmentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}