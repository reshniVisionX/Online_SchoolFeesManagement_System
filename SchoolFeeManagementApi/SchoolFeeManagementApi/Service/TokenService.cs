using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolFeeManagementApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolFeeManagementApi.Service
{
    public class TokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]!));
        }

        public string GenerateToken(Student user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.SId.ToString()),
                new Claim("admissionId", user.AdmissionId),
                new Claim(ClaimTypes.Role, user.Role.RoleName) 
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
             
                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
