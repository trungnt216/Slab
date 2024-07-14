using Microsoft.AspNetCore.Mvc;
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

        private readonly IWebHostEnvironment _env;

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        Subject subject = new Subject();

        private readonly bool _hasError = false;

        List<Subject> subjects = new List<Subject>();

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


            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetAll").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subjects = JsonConvert.DeserializeObject<List<Subject>>(data);
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

            if(userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
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

            TempData["subject_1"] = subjects.SingleOrDefault(item => item.ID == 6).SubjectName;
            TempData["subject_2"] = subjects.SingleOrDefault(item => item.ID == 7).SubjectName;
            TempData["subject_3"] = subjects.SingleOrDefault(item => item.ID == 8).SubjectName;
            TempData["subject_4"] = subjects.SingleOrDefault(item => item.ID == 9).SubjectName;
            TempData["subject_5"] = subjects.SingleOrDefault(item => item.ID == 10).SubjectName;
            TempData["subject_6"] = subjects.SingleOrDefault(item => item.ID == 11).SubjectName;
            TempData["subject_7"] = subjects.SingleOrDefault(item => item.ID == 12).SubjectName;
            TempData["subject_8"] = subjects.SingleOrDefault(item => item.ID == 13).SubjectName;
            TempData["subject_9"] = subjects.SingleOrDefault(item => item.ID == 14).SubjectName;
            TempData["subject_10"] = subjects.SingleOrDefault(item => item.ID == 15).SubjectName;
            TempData["subject_11"] = subjects.SingleOrDefault(item => item.ID == 16).SubjectName;
            TempData["subject_12"] = subjects.SingleOrDefault(item => item.ID == 17).SubjectName;
            TempData["subject_13"] = subjects.SingleOrDefault(item => item.ID == 18).SubjectName;
            TempData["subject_14"] = subjects.SingleOrDefault(item => item.ID == 19).SubjectName;
            TempData["subject_15"] = subjects.SingleOrDefault(item => item.ID == 20).SubjectName;
            TempData["subject_16"] = subjects.SingleOrDefault(item => item.ID == 21).SubjectName;
            TempData["subject_17"] = subjects.SingleOrDefault(item => item.ID == 22).SubjectName;
            TempData["subject_18"] = subjects.SingleOrDefault(item => item.ID == 23).SubjectName;
            TempData["subject_19"] = subjects.SingleOrDefault(item => item.ID == 24).SubjectName;
            TempData["subject_20"] = subjects.SingleOrDefault(item => item.ID == 25).SubjectName;
            TempData["subject_21"] = subjects.SingleOrDefault(item => item.ID == 26).SubjectName;
            TempData["subject_22"] = subjects.SingleOrDefault(item => item.ID == 27).SubjectName;
            TempData["subject_23"] = subjects.SingleOrDefault(item => item.ID == 28).SubjectName;
            TempData["subject_24"] = subjects.SingleOrDefault(item => item.ID == 29).SubjectName;
            TempData["subject_25"] = subjects.SingleOrDefault(item => item.ID == 30).SubjectName;
            TempData["subject_26"] = subjects.SingleOrDefault(item => item.ID == 31).SubjectName;

            return View();
        }

        [HttpGet]
        public ActionResult Information(int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            ViewData["layout"] = "~/Views/Chemistry/_LayoutChem.cshtml";

            ViewData["subjectID"] = subjectID;
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
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
            Program.jwtToken = null;
            return RedirectToAction("Login", "Login");
        }
    }
}
