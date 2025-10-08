using EnrollmentService.Data.Enum;

namespace EnrollmentService.Business.Dtos.Enrollment
{
    public record UpdateEnrollmentDto(EnrollmentStatus Status);
}