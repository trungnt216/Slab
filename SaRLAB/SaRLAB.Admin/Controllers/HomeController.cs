using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess;
using Microsoft.EntityFrameworkCore;
using SaRLAB.DataAccess.ProjectDto.LoginDto;
using SaRLAB.Models;



namespace SaRLAB.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            List<User>
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
