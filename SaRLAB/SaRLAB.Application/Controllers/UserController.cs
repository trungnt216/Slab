using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess;
using Microsoft.EntityFrameworkCore;
using SaRLAB.DataAccess.ProjectDto.LoginDto;


namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILoginDto _loginDto;

        public UserController(ILoginDto loginDto)
        {
            _loginDto = loginDto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_loginDto.GetAll());
        }

        [HttpGet]
        [Route("login/{email}/{passWord}")]
        public IActionResult LogIn(string email, string passWord)
        { 
            var user = _loginDto.LogIn(email, passWord);

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
