using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Services;

namespace SchoolFeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/student
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                var students = await _studentService.GetAllStudentsAsync();
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to fetch students. {ex.Message}" });
            }
        }

        // GET: api/student/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            try
            {
                var student = await _studentService.GetStudentByIdAsync(id);
                if (student == null)
                    return NotFound(new { message = $"Student with ID {id} not found." });

                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error while fetching student. {ex.Message}" });
            }
        }

        // POST: api/student
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid student data.", errors = ModelState });

            try
            {
                var student = await _studentService.AddStudentAsync(dto);
                return CreatedAtAction(nameof(GetStudentById), new { id = student.SId }, student);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to create student. {ex.Message}" });
            }
        }

        // PATCH: api/student/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdStudentsDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid update data.", errors = ModelState });

            try
            {
                var updated = await _studentService.UpdateStudentAsync(id, dto);
                if (updated == null)
                    return NotFound(new { message = $"Student with ID {id} not found." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error updating student. {ex.Message}" });
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterStudents(
          [FromQuery] string? name,
          [FromQuery] bool? isActive,
          [FromQuery] string? aid,
          [FromQuery] StudentCategory? category,
          [FromQuery] string? bloodGroup,
          [FromQuery] DateOnly? dob)
        {
            try
            {
                var students = await _studentService.FilterStudent(
                    name, isActive, aid, category, bloodGroup, dob
                );

                if (students == null || students.Count == 0)
                    return NotFound(new { message = "No students found matching the criteria." });

                return Ok(students);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = $"Invalid filter criteria: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error while filtering students. {ex.Message}" });
            }
        }

        // DELETE: api/student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var deleted = await _studentService.DeleteStudentAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Student with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error deleting student. {ex.Message}" });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByCourseId(int courseId)
        {
            var students = await _studentService.GetStudentsByCourseIdAsync(courseId);

            if (students == null || students.Count == 0)
                return NotFound($"No students found for CourseId {courseId}");

            return Ok(students);
        }

        [HttpPost("bulk-insert")]
        public async Task<IActionResult> BulkInsertStudents([FromBody] ICollection<StdClassDTO> students)
        {
            if (students == null || students.Count == 0)
                return BadRequest("Student list cannot be empty.");

            var (success, errorMessage) = await _studentService.InsertBulkStudentsAsync(students);

            if (!success)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = errorMessage ?? "Failed to insert students."
                });
            }

            return Ok(new
            {
                Success = true,
                Message = "Students inserted successfully."
            });
        }

        [HttpGet("credentials")]
        public async Task<ActionResult<IEnumerable<StudentCredentialDTO>>> GetAllUserCredentials()
        {
            var result = await _studentService.GetAllStudentsCredentialsAsync();
            return Ok(result);
        }
    }
}
