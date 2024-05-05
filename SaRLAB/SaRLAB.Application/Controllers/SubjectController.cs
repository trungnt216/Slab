using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.ProjectDto.LoginDto;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly ISubject _;

        public UserController(ILoginDto loginDto)
        {
            _loginDto = loginDto;
        }
    }
}
