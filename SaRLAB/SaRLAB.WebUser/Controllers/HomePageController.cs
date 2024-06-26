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

        Subject subject = new Subject();

        private readonly bool _hasError = false;

        Subject subject1 = new Subject();
        Subject subject2 = new Subject();
        Subject subject3 = new Subject();
        Subject subject4 = new Subject();
        Subject subject5 = new Subject();
        Subject subject6 = new Subject();

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


            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/6").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub2 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/7").Result;
            if (response_sub2.IsSuccessStatusCode)
            {
                string data = response_sub2.Content.ReadAsStringAsync().Result;
                subject2 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub3 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/8").Result;
            if (response_sub3.IsSuccessStatusCode)
            {
                string data = response_sub3.Content.ReadAsStringAsync().Result;
                subject3 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub4 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/9").Result;
            if (response_sub4.IsSuccessStatusCode)
            {
                string data = response_sub4.Content.ReadAsStringAsync().Result;
                subject4 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub5 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/10").Result;
            if (response_sub5.IsSuccessStatusCode)
            {
                string data = response_sub5.Content.ReadAsStringAsync().Result;
                subject5 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub6 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/11").Result;
            if (response_sub6.IsSuccessStatusCode)
            {
                string data = response_sub6.Content.ReadAsStringAsync().Result;
                subject6 = JsonConvert.DeserializeObject<Subject>(data);
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
                subjectFlag.BiologyPermissionFlag = true;
                subjectFlag.ChemistryPermissionFlag = true;
                subjectFlag.PhysicPermissionFlag = true;
                subjectFlag.MathPermissionFlag = true;
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

            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/6").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub2 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/7").Result;
            if (response_sub2.IsSuccessStatusCode)
            {
                string data = response_sub2.Content.ReadAsStringAsync().Result;
                subject2 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub3 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/8").Result;
            if (response_sub3.IsSuccessStatusCode)
            {
                string data = response_sub3.Content.ReadAsStringAsync().Result;
                subject3 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub4 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/9").Result;
            if (response_sub4.IsSuccessStatusCode)
            {
                string data = response_sub4.Content.ReadAsStringAsync().Result;
                subject4 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub5 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/10").Result;
            if (response_sub5.IsSuccessStatusCode)
            {
                string data = response_sub5.Content.ReadAsStringAsync().Result;
                subject5 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub6 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/11").Result;
            if (response_sub6.IsSuccessStatusCode)
            {
                string data = response_sub6.Content.ReadAsStringAsync().Result;
                subject6 = JsonConvert.DeserializeObject<Subject>(data);
            }
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            return View();
        }

        [HttpGet]
        public ActionResult Information(int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            if (subjectID == 1)
            {

                ViewData["layout"] = "~/Views/Chemistry/_LayoutChem.cshtml";
            }
            else if (subjectID == 3)
            {

                ViewData["layout"] = "~/Views/Biology/_Layout.cshtml";
            }
            else if (subjectID == 5)
            {

                ViewData["layout"] = "~/Views/Physics/_Layout.cshtml";
            }
            else if (subjectID == 2)
            {

                ViewData["layout"] = "~/Views/Math/_Layout.cshtml";
            }
            else if (subjectID == 6)
            {
                TempData["subject_1"] = subject1.SubjectName;
                ViewData["layout"] = "~/Views/Subject_1/_Layout.cshtml";
            }
            else if (subjectID == 7)
            {
                TempData["subject_1"] = subject2.SubjectName;
                ViewData["layout"] = "~/Views/Subject_2/_Layout.cshtml";
            }
            else if (subjectID == 8)
            {
                TempData["subject_1"] = subject3.SubjectName;
                ViewData["layout"] = "~/Views/Subject_3/_Layout.cshtml";
            }
            else if (subjectID == 9)
            {
                TempData["subject_1"] = subject4.SubjectName;
                ViewData["layout"] = "~/Views/Subject_4/_Layout.cshtml";
            }
            else if (subjectID == 10)
            {
                TempData["subject_1"] = subject5.SubjectName;
                ViewData["layout"] = "~/Views/Subject_5/_Layout.cshtml";
            }
            else if (subjectID == 11)
            {
                TempData["subject_1"] = subject6.SubjectName;
                ViewData["layout"] = "~/Views/Subject_6/_Layout.cshtml";
            }

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
