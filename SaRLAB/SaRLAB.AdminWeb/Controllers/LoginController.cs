using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.Text;

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

        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(LoginDto login) 
        {
            List<User> users = new List<User>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password).Result;

            Console.WriteLine(response.Content);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);

                TempData["success Message"] = "Login succes";
                return RedirectToAction("GetAll");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAll() 
        {
            List<User> users = new List<User>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            return View(users);

        }

        [HttpGet]
        public IActionResult Register() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user) 
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage respone;

                respone = _httpClient.PostAsync(_httpClient.BaseAddress + "User/register", content).Result;


                Console.WriteLine(respone.Content);

                if (respone.IsSuccessStatusCode)
                {
                    TempData["success Message"] = "User created";
                    return RedirectToAction("GetAll");
                }
            }
            catch (Exception ex)
            {
                TempData["Error Message"] = ex.Message;
                return View();
            }
            return View();
        }
        
    }
}
