using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SaRLAB.AdminWeb.Controllers
{
    public class ChemistryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        User userLogin = new User();

        public ChemistryController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = _configuration["JwtToken:Value"];

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(jwtToken);

            foreach (Claim claim in token.Claims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    userLogin.Email = claim.Value;
                }
                else if (claim.Type == ClaimTypes.Role)
                {
                    
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BannerConfig()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            List<UserDto> users = new List<UserDto>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAll").Result;

            Console.WriteLine(_httpClient.BaseAddress + "User/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserDto>>(data);
            }

            return View(users);

        }

        public IActionResult GetUsedByRole()
        {
            return View();
        }

        [HttpGet]
        public IActionResult InsertUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertUser(User user) {
            if(user == null) 
            { 
                return View();
            }
            try 
            {
                user.CreateBy = userLogin.Email;
                user.UpdateBy = userLogin.Email;
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/register", content).Result;

                if(response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAllUser");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Edit(UserDto user, string email)
        {
            Console.WriteLine(email);
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        public IActionResult PostManagement()
        {
            return View();
        }


    }
}
