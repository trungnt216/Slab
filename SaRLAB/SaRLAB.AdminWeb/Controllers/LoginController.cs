using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using NuGet.Protocol;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using SaRLAB.AdminWeb.Models;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SaRLAB.AdminWeb.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
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

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginDto login)
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

                /*                Uri newbaseAddress = new Uri(_httpClient.BaseAddress + "?jwt=" + jwtToken);

                                _httpClient.BaseAddress = newbaseAddress;*/

                DecodeJwtToken(jwtToken);

                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["Error"] = "Wrong credentials. Please, try again!";
                return View("Index");
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
