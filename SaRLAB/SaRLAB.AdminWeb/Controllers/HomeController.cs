 using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaRLAB.AdminWeb.Models;
using SaRLAB.Models.Entity;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SaRLAB.AdminWeb.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri(Program.api);

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        Subject subject1 = new Subject();

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/6").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Subject> subjects = new List<Subject>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetAll").Result;

            Console.WriteLine(response.StatusCode);

            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                subjects = JsonConvert.DeserializeObject<List<Subject>>(data);
            }

            TempData["subject_1"] = subject1.SubjectName;

            return View(subjects);
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
