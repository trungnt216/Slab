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
        string pathFolderSave = Program.FilePath;

        Uri baseAddress = new Uri(Program.api);
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        private readonly bool _queFlag = false;

        SubjectFlag subjectFlag = new SubjectFlag();

        public MultipleQuestionsController(ILogger<HomePageController> logger, IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            if (jwtToken == null)
            {
                _hasError = true;
                return;
            }

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

            HttpResponseMessage response_sub = _httpClient.GetAsync(_httpClient.BaseAddress + "SubjectFlag/GetByID/" + userLogin.Email).Result;

            if (response_sub.IsSuccessStatusCode)
            {
                string data = response_sub.Content.ReadAsStringAsync().Result;
                subjectFlag = JsonConvert.DeserializeObject<SubjectFlag>(data);
            }


            if (userLogin.RoleName == "Owner" || userLogin.RoleName == "Admin" || userLogin.RoleName == "Teacher" || userLogin.RoleName == "Technical")
            {
                _queFlag = true;
                return;
            }

        }


        [HttpGet]
        public IActionResult GetAllQuestion()
        {
            if(_queFlag)
            {
                return RedirectToAction("index", "Chemistry");
            }

            if (subjectFlag.ChemistryPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/1").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Chemistry");
            }

            if (subjectFlag.ChemistryPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/1/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetAllQuestion_Bio()
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Biology");
            }

            if (subjectFlag.BiologyPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/3").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Bio(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Biology");
            }

            if (subjectFlag.BiologyPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/3/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetAllQuestion_Physics()
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Physics");
            }

            if (subjectFlag.PhysicPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/5").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Physics(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Physics");
            }

            if (subjectFlag.PhysicPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/5/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetAllQuestion_Math()
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Math");
            }

            if (subjectFlag.MathPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/2").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Math(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index",  "Math");
            }

            if (subjectFlag.MathPermissionFlag == false)
            {
                return View("Error");
            }

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/2/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

    }
}
