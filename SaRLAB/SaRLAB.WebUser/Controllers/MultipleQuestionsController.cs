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

        private readonly IHttpContextAccessor _httpContextAccessor;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        private readonly bool _queFlag = false;

        SubjectFlag subjectFlag = new SubjectFlag();

        public MultipleQuestionsController(ILogger<HomePageController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var httpContext = _httpContextAccessor.HttpContext;
            var jwtToken = httpContext.Session.GetString("jwtToken");

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

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject1()
        {
            if (_queFlag == true || subjectFlag.BackupSubject1MarkFlag == true) 
            {
                return RedirectToAction("index", "Subject_1");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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
        public IActionResult GetQuestionRepeat_Subject1(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_1");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject2()
        {
            if (_queFlag || subjectFlag.BackupSubject2MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_2");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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
        public IActionResult GetQuestionRepeat_Subject2(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_2");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject3()
        {
            if (_queFlag || subjectFlag.BackupSubject3MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_3");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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
        public IActionResult GetQuestionRepeat_Subject3(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_3");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject4()
        {
            if (_queFlag || subjectFlag.BackupSubject4MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_4");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/4").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject4(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_4");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/4/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject5()
        {
            if (_queFlag || subjectFlag.BackupSubject5MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_5");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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
        public IActionResult GetQuestionRepeat_Subject5(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_5");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject6()
        {
            if (_queFlag || subjectFlag.BackupSubject6MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_6");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/6").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject6(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_6");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/6/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject7()
        {
            if (_queFlag || subjectFlag.BackupSubject7MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_7");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/7").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject7(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_7");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/7/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject8()
        {
            if (_queFlag || subjectFlag.BackupSubject8MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_8");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/8").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject8(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_8");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/8/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject9()
        {
            if (_queFlag || subjectFlag.BackupSubject9MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_9");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/9").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject9(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_9");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/9/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject10()
        {
            if (_queFlag || subjectFlag.BackupSubject10MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_10");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/10").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject10(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_10");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/10/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject11()
        {
            if (_queFlag || subjectFlag.BackupSubject11MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_11");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/11").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject11(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_11");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/11/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject12()
        {
            if (_queFlag || subjectFlag.BackupSubject12MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_12");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/12").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject12(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_12");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/12/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject13()
        {
            if(_queFlag || subjectFlag.BackupSubject13MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_13");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/13").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject13(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_13");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/13/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject14()
        {
            if (_queFlag || subjectFlag.BackupSubject14MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_14");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/14").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject14(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_14");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/14/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject15()
        {
            if (_queFlag || subjectFlag.BackupSubject15MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_15");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/15").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject15(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_15");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/15/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }


        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject16()
        {
            if (_queFlag || subjectFlag.BackupSubject16MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_16");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/16").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject16(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_16");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/16/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject17()
        {
            if (_queFlag || subjectFlag.BackupSubject17MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_17");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/17").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject17(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_17");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/17/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject18()
        {
            if (_queFlag || subjectFlag.BackupSubject18MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_18");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/18").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject18(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_18");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/18/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject19()
        {
            if (_queFlag || subjectFlag.BackupSubject19MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_19");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/19").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject19(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_19");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/19/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject20()
        {
            if (_queFlag || subjectFlag.BackupSubject20MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_20");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/20").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject20(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_20");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/20/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject21()
        {
            if (_queFlag || subjectFlag.BackupSubject21MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_21");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/21").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject21(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_21");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/21/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject22()
        {
            if (_queFlag || subjectFlag.BackupSubject22MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_22");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/22").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject22(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_22");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/22/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject23()
        {
            if (_queFlag || subjectFlag.BackupSubject23MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_23");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/23").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject23(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_23");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/23/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject24()
        {
            if (_queFlag || subjectFlag.BackupSubject24MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_24");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/24").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject24(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_24");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/24/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject25()
        {
            if (_queFlag || subjectFlag.BackupSubject25MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_25");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/25").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject25(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_25");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/25/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject26()
        {
            if (_queFlag || subjectFlag.BackupSubject26MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_26");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/26").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject26(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_26");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/26/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject27()
        {
            if (_queFlag || subjectFlag.BackupSubject27MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_27");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/27").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject27(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_27");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/27/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject28()
        {
            if (_queFlag || subjectFlag.BackupSubject28MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_28");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/28").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject28(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_28");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/28/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }
        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject29()
        {
            if (_queFlag || subjectFlag.BackupSubject29MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_29");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/29").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject29(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_29");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/29/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        //-------------------------------------

        [HttpGet]
        public IActionResult GetAllQuestion_Subject30()
        {
            if (_queFlag || subjectFlag.BackupSubject30MarkFlag == true)
            {
                return RedirectToAction("index", "Subject_30");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/30").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult GetQuestionRepeat_Subject30(int count)
        {
            if (_queFlag)
            {
                return RedirectToAction("index", "Subject_30");
            }

            if (subjectFlag.BackupSubject1PermissionFlag == false)
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzesAfterDone/" + userLogin.SchoolId + "/30/" + count).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            return View(equipment);
        }


    }
}
