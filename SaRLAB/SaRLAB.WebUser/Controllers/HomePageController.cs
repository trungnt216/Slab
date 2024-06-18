using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SaRLAB.UserWeb.Controllers
{
    public class HomePageController : Controller
    {
        string pathFolderSave = null;

        Uri baseAddress = new Uri(Program.api);

        private readonly IWebHostEnvironment _env;

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        private readonly bool _hasError = false;

        public HomePageController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            pathFolderSave = _configuration["PathFolder:Value"];

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(jwtToken);

            if (jwtToken == null)
            {
                _hasError = true;
                return; // Early exit from constructor
            }

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

        }


        public ActionResult Index()
        {
            ViewBag.ActiveMenu = "homePage";

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            return View();
        }

        public ActionResult Home()
        {
            SubjectFlag subjectFlag = new SubjectFlag();
            HttpResponseMessage response_sub = _httpClient.GetAsync(_httpClient.BaseAddress + "SubjectFlag/GetByID/" + userLogin.Email).Result;
            if (response_sub.IsSuccessStatusCode)
            {
                string data = response_sub.Content.ReadAsStringAsync().Result;
                subjectFlag = JsonConvert.DeserializeObject<SubjectFlag>(data);
            }

            ViewBag.SubjectFlag = subjectFlag;

            School school = new School();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetByID/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                school = JsonConvert.DeserializeObject<School>(data);
            }

            ViewBag.school = school;

            return View();
        }

        [HttpGet]
        public ActionResult Information()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + userLogin.Email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "homePage";
            return View(users);
        }

        [HttpGet]
        public ActionResult Edit_Information(string email)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + userLogin.Email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "homePage";
            return View(users);
        }
        [HttpPost]
        public ActionResult Edit_Information(User user, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/User");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }
                user.AvtPath = pathFolderSave + "FileFolder/User/" + uniqueFileName;
            }

            try
            {
                user.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "homePage";
                    return RedirectToAction("Information");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "homePage";
                return View();
            }
            ViewBag.ActiveMenu = "homePage";
            return View();
        }


        public IActionResult Logout()
        {
            Program.jwtToken = null;
            return RedirectToAction("Login", "Login");
        }
    }
}
