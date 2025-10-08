using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolFeeManagementApi.Services;

namespace SchoolFeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SchoolController : ControllerBase
    {
        private readonly SchoolService _schoolService;

        public SchoolController(SchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        // GET: api/school
        [HttpGet]
        public async Task<IActionResult> GetAllSchools()
        {
            try
            {
                var schools = await _schoolService.GetAllSchoolsAsync();

                if (schools == null || !schools.Any())
                    return NotFound(new { message = "No schools found." });

                return Ok(schools);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error fetching schools: {ex.Message}" });
            }
        }
    }
}
