using EnrollmentService.Business.Dtos.Enrollment;
using EnrollmentService.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnrollmentService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
    {
        #region Services

        private readonly IEnrollmentService _enrollmentService = enrollmentService;

        #endregion Services

        #region APIS

        /// <summary>
        /// Obtiene todas las inscripciones
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllAsync();
            return Ok(enrollments);
        }

        /// <summary>
        /// Obtiene una inscripción por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnrollmentDto>> GetById(Guid id)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(id);

            if (enrollment == null)
                return NotFound(new { message = $"Enrollment {id} not found" });

            return Ok(enrollment);
        }

        /// <summary>
        /// Crea una nueva inscripción
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EnrollmentDto>> Create([FromBody] CreateEnrollmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var enrollment = await _enrollmentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = enrollment.Id },
                enrollment);
        }

        /// <summary>
        /// Actualiza una inscripción existente
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EnrollmentDto>> Update(
            Guid id,
            [FromBody] UpdateEnrollmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var enrollment = await _enrollmentService.UpdateAsync(id, dto);

            if (enrollment == null)
                return NotFound(new { message = $"Enrollment {id} not found" });

            return Ok(enrollment);
        }

        /// <summary>
        /// Elimina una inscripción por ID
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _enrollmentService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = $"Enrollment {id} not found" });

            return NoContent();
        }

        #endregion APIS
    }
}