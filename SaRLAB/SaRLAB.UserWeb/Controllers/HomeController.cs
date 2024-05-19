using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SaRLAB.UserWeb.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;

namespace SaRLAB.UserWeb.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;

        public HomeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
        }

        public void DecodeJwtToken(string jwtToken)
        {
            // Create a JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Parse the JWT token
            var token = tokenHandler.ReadJwtToken(jwtToken);

            // Access the claims from the JWT token
            /*            foreach (Claim claim in token.Claims)
                        {
                            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                        }*/

            foreach (Claim claim in token.Claims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    Console.WriteLine($"Email: {claim.Value}");
                }
                else if (claim.Type == ClaimTypes.Role)
                {
                    Console.WriteLine($"Role: {claim.Value}");
                }
            }
        }

        //---------------------------------------login---------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User login)
        {
            List<LoginDto> users = new List<LoginDto>();


            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password).Result;

            Console.WriteLine(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password);

            Console.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {

                string jwtToken = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(jwtToken);

                Console.WriteLine(_configuration["jwtToken:Value"]);

                _configuration["JwtToken:Value"] = jwtToken;

                Console.WriteLine(_configuration["jwtToken:Value"]);

                DecodeJwtToken(jwtToken);

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.ReadJwtToken(jwtToken);

                foreach (Claim claim in token.Claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        Console.WriteLine(claim.Value);
                        if (!claim.Value.Equals("Admin") && !claim.Value.Equals("Owner"))
                        {
                            TempData["Error"] = "Tài khoản này không có quyền truy cập. Vui lòng thử lại!";
                            return View();
                        }
                    }
                }

                /* return RedirectToAction("Index", "Home");*/
                return RedirectToAction("GetAllBanner", "Configuration");
            }
            else
            {
                TempData["Error"] = "Không có tài khoản. Vui lòng thử lại!";
                return View();
            }
        }

        //----------------------------register-------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
