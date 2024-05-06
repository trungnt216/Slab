using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;

namespace SaRLAB.AdminWeb.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        public LoginController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        public IActionResult Login(LoginDto login)
        {

            List<LoginDto> users = new List<LoginDto>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password).Result;

            Console.WriteLine(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password);

            if (response.IsSuccessStatusCode)
            {
                /*                    string data = response.Content.ReadAsStringAsync().Result;
                                    users = JsonConvert.DeserializeObject<List<LoginDto>>(data);*/
                return RedirectToAction("GetAll");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<User> users = new List<User>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAll").Result;

            Console.WriteLine(_httpClient.BaseAddress + "User/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            return View(users);

        }
    }
}
