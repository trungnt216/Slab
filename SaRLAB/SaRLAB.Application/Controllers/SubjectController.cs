using Microsoft.AspNetCore.Mvc;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
