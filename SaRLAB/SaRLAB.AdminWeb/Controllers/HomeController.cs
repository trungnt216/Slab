using Microsoft.AspNetCore.Mvc;
using SaRLAB.AdminWeb.Models;
using System.Diagnostics;

namespace SaRLAB.AdminWeb.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
