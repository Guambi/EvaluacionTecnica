using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Business.Interfaces;
using EnrollmentService.Data.Entities;
using EnrollmentService.Data.Enum;
using EnrollmentService.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnrollmentService.Business.Implementation
{
    public class EnrollmentService(ApplicationDbContext context, ILogger<EnrollmentService> logger) : IEnrollmentService
    {
        #region Services

        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<EnrollmentService> _logger = logger;

        #endregion Services

        #region Public Methods

        public async Task<EnrollmentDto?> GetByIdAsync(Guid id)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == id);

            return enrollment == null ? null : MapToDto(enrollment);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetAllAsync()
        {
            var enrollments = await _context.Enrollments
                .ToListAsync();

            return enrollments.Select(MapToDto);
        }

        public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentDto dto)
        {
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = dto.StudentId,
                Status = EnrollmentStatus.Pending,
                Amount = dto.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Enrollment created: {EnrollmentId} for student {StudentId}",
                enrollment.Id, enrollment.StudentId);

            return MapToDto(enrollment);
        }

        public async Task<EnrollmentDto?> UpdateAsync(Guid id, UpdateEnrollmentDto dto)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return null;

            enrollment.Status = dto.Status;
            enrollment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Enrollment updated: {EnrollmentId} to status {Status}",
                enrollment.Id, enrollment.Status);

            return MapToDto(enrollment);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Enrollment deleted: {EnrollmentId}", id);
            return true;
        }

        
        #endregion Public Methods

        #region Private Methods

        private static EnrollmentDto MapToDto(Enrollment enrollment)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                Status = enrollment.Status,
                Amount = enrollment.Amount,
                CreatedAt = enrollment.CreatedAt,
                UpdatedAt = enrollment.UpdatedAt
            };
        }

        #endregion Private Methods
    }
}