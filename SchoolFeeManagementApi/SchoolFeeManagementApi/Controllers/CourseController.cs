using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolFeeManagementApi.DTOs;
using SchoolFeeManagementApi.Models;
using SchoolFeeManagementApi.Service;

namespace SchoolFeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // ✅ POST: api/course
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CourseDTO dto)
        {
            try
            {
                var result = await _courseService.AddCourseAsync(dto);
                return Ok(new { message = "Course created successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ PATCH: api/course/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] UpdCourseDTO dto)
        {
            try
            {
                var result = await _courseService.UpdateCourseAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = "Course not found" });

                return Ok(new { message = "Course updated successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ DELETE (Soft Delete): api/course/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                var result = await _courseService.DeleteCourseAsync(id);
                if (!result)
                    return NotFound(new { message = "Course not found" });

                return Ok(new { message = "Course deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ GET: api/course
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var result = await _courseService.GetAllCoursesAsync();
                if (result == null || !result.Any())
                    return NotFound(new { message = "No courses found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ GET: api/course/filter?name=xyz
        [HttpGet("filter")]
        public async Task<IActionResult> FilterCourse([FromQuery] string? name, [FromQuery] bool active)
        {
            try
            {
                var result = await _courseService.FilterCourseAsync(name, active);
                if (result == null || !result.Any())
                    return NotFound(new { message = "No courses found matching the filter" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("toggle-status/{id}")]
        public async Task<ActionResult<bool>> ToggleCourseStatus(int id)
        {
            try
            {
                var result = await _courseService.ToggleCourseStatus(id);
                if (!result)
                    return NotFound(new { message = "Course not found" });

                return Ok(new { message = "Course status toggled successfully" });
            }
            catch (Exception ex)
            {
                  return StatusCode(500, new { message = "An error occurred while toggling course status.", error = ex.Message });
            }
        }
        [HttpGet("Id/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid course ID.");
            }

            var course = await _courseService.GetCourseById(id);

            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            return Ok(course);
        }

    }
}
