using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using SaRLAB.Models.Entity;
using Newtonsoft.Json;

namespace SaRLAB.UserWeb.Controllers
{
    public class MultipleQuestionsController : Controller
    {
        string pathFolderSave = "https://localhost:7135//uploads/";

        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        public MultipleQuestionsController(ILogger<HomePageController> logger, IConfiguration configuration)
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
                    userLogin.RoleName = claim.Value;
                }
                else if (claim.Type == "SchoolId")
                {
                    userLogin.SchoolId = int.Parse(claim.Value);
                }
                else if (claim.Type == "Name")
                {
                    userLogin.Name = claim.Value;
                }
                else if (claim.Type == "avt")
                {
                    userLogin.AvtPath = claim.Value;
                }
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            if (userLogin.RoleName == ("Admin"))
            {
                checkRole = 1;
            }

        }


        [HttpGet]
        public IActionResult GetAllQuestion()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/CHEMISTRYE").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }
    }
}
