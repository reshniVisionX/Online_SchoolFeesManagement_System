using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SchoolFeeManagementApi.Interface;
using System.Text;
using SchoolFeeManagementApi.Service;
using SchoolFeeManagementApi.DTOs;

namespace SchoolFeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly SymmetricSecurityKey _key;
        private readonly TokenService _tokenService;
        private readonly IStudent _userSer;
        public UserController(IConfiguration configuration, IStudent user, TokenService tokenService)
        {
            _key = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(configuration["TokenKey"]!)
           );
            _tokenService = tokenService;
            _userSer = user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var user = await _userSer.GetStudentByAdmissionId(loginDto.AdmissionId);

            if (user == null) return Unauthorized("Invalid username");

            Boolean isValidPassword = await _userSer.ValidatePassword(user.SId,loginDto.Password);

            if (!isValidPassword) return Unauthorized("Invalid password");

            var token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                token,
                username=user.SName,
                userId = user.SId,
                admissionId = user.AdmissionId,
                role = user.Role.RoleName
            });
        }


    }
}
