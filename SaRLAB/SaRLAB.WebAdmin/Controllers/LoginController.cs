using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.ProjectDto.LoginDto;

namespace SaRLAB.WebAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginDto _loginDto;

        public LoginController(ILoginDto loginDto)
        {
            _loginDto = loginDto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_loginDto.GetAll());
        }
    }
}
