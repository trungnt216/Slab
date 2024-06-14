using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace SaRLAB.UserWeb.Controllers
{
    public class LibraryController : Controller
    {
        string pathFolderSave = null;

        private readonly IWebHostEnvironment _env;
        Uri baseAddress = new Uri("http://api.sarlabeducation.com/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        public LibraryController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            if(jwtToken == null){
                _hasError = true;
                return;
            }

            pathFolderSave = _configuration["PathFolder:Value"];

            Console.WriteLine(pathFolderSave);

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

        //------------------------------- thư viện Library ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Library()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllDocumentBySchoolId/" + userLogin.SchoolId).Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
                documents = documents.Where(doc => doc.PageFlag == true).ToList();
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(documents);
        }

        [HttpGet]
        public ActionResult Details_Library(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(document);
        }
    }
}
