using EnrollmentService.Business.Dtos.Enrollment;

namespace EnrollmentService.Business.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto?> GetByIdAsync(Guid id);

        Task<IEnumerable<EnrollmentDto>> GetAllAsync();

        Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto dto);

        Task<EnrollmentDto?> UpdateAsync(Guid id, UpdateEnrollmentDto dto);

        Task<bool> DeleteAsync(Guid id);
    }
}