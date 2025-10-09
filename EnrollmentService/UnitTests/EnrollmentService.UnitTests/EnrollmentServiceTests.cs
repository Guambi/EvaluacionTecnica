using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Data.Entities;
using EnrollmentService.Data.Enum;
using EnrollmentService.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EnrollmentService.UnitTests
{
    public class EnrollmentServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<Business.Implementation.EnrollmentService>> _loggerMock;
        private readonly Business.Implementation.EnrollmentService _service;

        public EnrollmentServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<Business.Implementation.EnrollmentService>>();
            _service = new Business.Implementation.EnrollmentService(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateEnrollment_WithPendingStatus()
        {
            // Arrange
            var dto = new CreateEnrollmentDto("STU001", 1500);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("STU001", result.StudentId);
            Assert.Equal(1500.00m, result.Amount);
            Assert.Equal(EnrollmentStatus.Pending, result.Status);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEnrollment_WhenExists()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU002",
                Status = EnrollmentStatus.Pending,
                Amount = 2000.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(enrollment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(enrollment.Id, result.Id);
            Assert.Equal("STU002", result.StudentId);
            Assert.Equal(2000.00m, result.Amount);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEnrollments()
        {
            // Arrange
                var enrollments = new List<Enrollment>
            {
                new() { Id = Guid.NewGuid(), StudentId = "STU001", Status = EnrollmentStatus.Pending, Amount = 1000m, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), StudentId = "STU002", Status = EnrollmentStatus.Active, Amount = 2000m, CreatedAt = DateTime.UtcNow },
                new() { Id = Guid.NewGuid(), StudentId = "STU003", Status = EnrollmentStatus.Completed, Amount = 3000m, CreatedAt = DateTime.UtcNow }
            };

            _context.Enrollments.AddRange(enrollments);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(result, e => e.StudentId == "STU001");
            Assert.Contains(result, e => e.StudentId == "STU002");
            Assert.Contains(result, e => e.StudentId == "STU003");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateStatus_AndSetUpdatedAt()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU003",
                Status = EnrollmentStatus.Pending,
                Amount = 1000.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateEnrollmentDto(EnrollmentStatus.Active);

            // Act
            var result = await _service.UpdateAsync(enrollment.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EnrollmentStatus.Active, result.Status);
            Assert.NotNull(result.UpdatedAt);
            Assert.True(result.UpdatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenEnrollmentNotExists()
        {
            // Arrange
            var updateDto = new UpdateEnrollmentDto(EnrollmentStatus.Active);

            // Act
            var result = await _service.UpdateAsync(Guid.NewGuid(), updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = "STU004",
                Status = EnrollmentStatus.Pending,
                Amount = 1500.00m,
                CreatedAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAsync(enrollment.Id);

            // Assert
            Assert.True(result);
            var deleted = await _context.Enrollments.FindAsync(enrollment.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
        {
            // Act
            var result = await _service.DeleteAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}