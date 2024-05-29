using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SaRLAB.DataAccess.Dto.LoginService;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginDto;
        private readonly IConfiguration _configuration;
        public LoginController(ILoginService loginDto, IConfiguration configuration)
        {
            _loginDto = loginDto;
            _configuration = configuration;
        }


        [HttpGet]
        [Route("test")]
        [Authorize(Roles = "Admin")]
        public IActionResult test()
        {
            return Ok(_loginDto.GetAll());
        }


        [HttpGet]
        [Route("login/{email}/{passWord}")]
        public IActionResult LogIn(string email, string passWord)
        {
            var user =  _loginDto.Login(email, passWord);

            if (user == null)
            {
                return NotFound();
            }
            var userRole = user.RoleName;
            string schoolId = user.SchoolId.ToString();
            var authuClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.Email),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("SchoolId", schoolId),
                new Claim("Name", user.Name),
                new Claim("avt", user.AvtPath),
                new Claim("JWTID",Guid.NewGuid().ToString()),

            };

            return Ok(GenerateNewJsonWebToken(authuClaims));
            
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}
