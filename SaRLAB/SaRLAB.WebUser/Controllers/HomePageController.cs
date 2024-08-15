using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace SaRLAB.UserWeb.Controllers
{
    public class HomePageController : Controller
    {
        string pathFolderSave = null;

        Uri baseAddress = new Uri(Program.api);

        string password = null;

        private readonly IWebHostEnvironment _env;

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IHttpContextAccessor _httpContextAccessor;

        UserDto userLogin = new UserDto();

        Subject subject = new Subject();

        private readonly bool _hasError = false;

        List<Subject> subjects = new List<Subject>();

        List<ManageTitle> manageTitles = new List<ManageTitle>();

        public HomePageController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            
            var httpContext = _httpContextAccessor.HttpContext;
            var jwtToken = httpContext.Session.GetString("jwtToken");
            password = httpContext.Session.GetString("pass");

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


            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetSubjectBySchool/" + userLogin.SchoolId).Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subjects = JsonConvert.DeserializeObject<List<Subject>>(data);
            }

            HttpResponseMessage response_title = _httpClient.GetAsync(_httpClient.BaseAddress + "ManageTitle/GetManageTitlesAccordingSchool/" + userLogin.SchoolId).Result;
            if (response_title.IsSuccessStatusCode)
            {
                string data = response_title.Content.ReadAsStringAsync().Result;
                manageTitles = JsonConvert.DeserializeObject<List<ManageTitle>>(data);
            }
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
            ViewBag.MenuItems = manageTitles;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["School"] = userLogin.SchoolId;

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

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                Type type = subjectFlag.GetType();

                // Set all boolean properties to true
                foreach (PropertyInfo property in type.GetProperties())
                {
                    if (property.PropertyType == typeof(bool?) && property.CanWrite)
                    {
                        property.SetValue(subjectFlag, true);
                    }
                }

                // Set all boolean fields to true
                foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (field.FieldType == typeof(bool?))
                    {
                        field.SetValue(subjectFlag, true);
                    }
                }
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

            for (int i = 1; i <= 30; i++)
            {
                var subjectname = subjects.SingleOrDefault(item => item.Type == i);
                if (subjectname != null)
                {
                    TempData[$"subject_{i}"] = subjectname.SubjectName;
                }
            }

/*            TempData["type"] = type;*/
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath;

            return View();
        }

        public ActionResult HomeSchool()
        {
            if (userLogin.RoleName != "Owner")
            {
                return RedirectToAction("Index", "HomePage");
            }

            List<School> schools = new List<School>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetAllSchool").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                schools = JsonConvert.DeserializeObject<List<School>>(data);
            }

            ViewBag.schoolItems = schools;

            return View();
        }

        public ActionResult ChangeSchool(int SchoolId)
        {
            StringContent content = new StringContent("", Encoding.UTF8, "application/json");

            HttpResponseMessage response2 = _httpClient.PostAsync(_httpClient.BaseAddress + "User/UpdateSchool/" + userLogin.Email + "/" + SchoolId, content).Result;

            HttpResponseMessage response3;
            response3 = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + userLogin.Email + "/" + password).Result;

            string jwtToken1 = response3.Content.ReadAsStringAsync().Result;


            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Session.SetString("jwtToken", jwtToken1);
            /*            Program.jwtToken = jwtToken1;*/

            return RedirectToAction("Index", "HomePage");
        }

        [HttpGet]
        public ActionResult Information(int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            ViewData["layout"] = "~/Views/Subject_" + subjectID + "/_Layout.cshtml";
            Subject subject1 = new Subject();
            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetSubjectBySchoolAndType/" + userLogin.SchoolId + "/" + subjectID).Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }
            TempData["subject_1"] = subject1.SubjectName;
            ViewData["subjectID"] = subjectID;
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            ViewBag.MenuItems = manageTitles;
            TempData["AvtPath"] = userLogin.AvtPath;
            School school = new School();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetByID/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                school = JsonConvert.DeserializeObject<School>(data);
            }

            TempData["School"] = school.Name;
            User users = new User();

            HttpResponseMessage response1 = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + userLogin.Email).Result;

            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "homePage";
            return View(users);
        }

        [HttpGet]
        public ActionResult Edit_Information(string email, int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            ViewBag.MenuItems = manageTitles;
            TempData["AvtPath"] = userLogin.AvtPath;
            ViewData["subjectID"] = subjectID;


            ViewData["layout"] = "~/Views/Chemistry/_LayoutChem.cshtml";


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
        public ActionResult Edit_Information(User user, IFormFile File, int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            ViewBag.MenuItems = manageTitles;
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
                    return RedirectToAction("Information", new { subjectID = subjectID });
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
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
