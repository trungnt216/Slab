using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Dto.LoginService;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginDto;

        public LoginController(ILoginService loginDto)
        {
            _loginDto = loginDto;
        }


        [HttpGet]
        [Route("test")]
        public IActionResult test()
        {
            return Ok(_loginDto.GetAll());
        }


        [HttpGet]
        [Route("login/{email}/{passWord}")]
        public IActionResult LogIn(string email, string passWord)
        {
            var user = _loginDto.Login(email, passWord);

            if (user != null)
            {
                // Return a successful response with the user object
                return Ok(user);
            }
            else
            {
                // If user is not found, return a 404 Not Found response
                return NotFound();
            }
        }
    }
}
