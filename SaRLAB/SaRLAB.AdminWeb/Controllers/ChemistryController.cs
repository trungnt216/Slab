using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Newtonsoft.Json;

namespace SaRLAB.AdminWeb.Controllers
{
    public class ChemistryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

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
                    userLogin.RoleName = claim.Value;
                }
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }


        [HttpGet]
        public IActionResult GetAll_ScientificResearch()
        {
            List<ScientificResearch> scientificResearches = new List<ScientificResearch>();
            
            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "ScientificResearch/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                scientificResearches = JsonConvert.DeserializeObject<List<ScientificResearch>>(data);
            }

            return View(scientificResearches);
        }
    }
}
