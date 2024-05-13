using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "ScientificResearch/1/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                scientificResearches = JsonConvert.DeserializeObject<List<ScientificResearch>>(data);
            }

            return View(scientificResearches);
        }

        [HttpGet]
        public IActionResult Edit_TopicScientificResearch()
        {
            return View();
        }

        public IActionResult Delete_TopicScientificResearch(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "ScientificResearch/Delete/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAll_ScientificResearch");
        }

        [HttpGet]
        public IActionResult Create_TopicScientificResearch()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create_TopicScientificResearch(ScientificResearch scientificResearch) 
        {
            if(scientificResearch == null)
            {
                return View();
            }

            try
            {
                scientificResearch.CreateBy = userLogin.Email;
                scientificResearch.CreateTime = DateTime.Now;
                scientificResearch.UpdateTime = DateTime.Now;
                scientificResearch.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(scientificResearch);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "ScientificResearch/1/Insert", content).Result;

                if(response.IsSuccessStatusCode) 
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Create_ScientificResearch()
        {
            return View();
        }

        public IActionResult Browse_TopicScientificResearch()
        {
            return View();
        }

        public IActionResult GetAll_PaperResearch()
        {
            return View();
        }

        public IActionResult Create_PaperResearch()
        {
            return View();
        }

        public IActionResult Browse_PaperResearch()
        {
            return View();
        }

        public IActionResult GetAll_Equipment()
        {
            return View();
        }

        public IActionResult Create_Equipment()
        {
            return View();
        }

        public IActionResult GetAll_PlanDetail() 
        {
            return View();        
        }

        public IActionResult Create_PlanDetail()
        {
            return View();
        }

        public IActionResult GetAll_PracticePlan()
        {
            return View();
        }

        public IActionResult Safety_PracticePlan()
        {
            return View();
        }

        public IActionResult Point_PracticePlan()
        {
            return View();
        }
    }
}
