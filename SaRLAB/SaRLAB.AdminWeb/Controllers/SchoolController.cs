using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SaRLAB.AdminWeb.Controllers
{
    public class SchoolController : Controller
    {
        string pathFolderSave = Program.FilePath;

        Uri baseAddress = new Uri(Program.api);

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;


        UserDto userLogin = new UserDto();

        Subject subject1 = new Subject();
        Subject subject2 = new Subject();
        Subject subject3 = new Subject();
        Subject subject4 = new Subject();
        Subject subject5 = new Subject();
        Subject subject6 = new Subject();

        public SchoolController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;


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

        //------------------------------------------------- school ------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult GetAllSchool()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            List<School> schools = new List<School>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetAllSchool").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                schools = JsonConvert.DeserializeObject<List<School>>(data);
            }
            ViewBag.ActiveMenu = "school";
            return View(schools);
        }

        [HttpGet]
        public ActionResult CreateSchool()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            if (userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "school";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "school";
                TempData["notice"] = "Bạn không có quyền chỉnh sửa";
                return RedirectToAction("GetAllSchool", "School");
            }
        }
        [HttpPost]
        public ActionResult CreateSchool(School school)
        {
            try
            {

                string data = JsonConvert.SerializeObject(school);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "School/Insert", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ActiveMenu = "school";
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAllSchool", "School");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "school";
            return View();
        }


        [HttpGet]
        public ActionResult EditSchool(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            School school = new School();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetByID/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                school = JsonConvert.DeserializeObject<School>(data);
            }

            if (school == null)
            {
                TempData["notice"] = "không tìm thấy dữ liệu";
                ViewBag.ActiveMenu = "school";
                return Ok();
            }

            if (userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "school";
                return View(school);
            }
            else
            {
                ViewBag.ActiveMenu = "school";
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                return RedirectToAction("GetAllSchool", "School");
            }
        }

        [HttpPost]
        public ActionResult EditSchool(School school)
        {
            try
            {

                string data = JsonConvert.SerializeObject(school);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "School/Update/" + school.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ActiveMenu = "school";
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAllSchool", "School");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "school";
            return View();
        }


        public ActionResult DeleteSchool(int id)
        {

            if (userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "School/Delete/" + id, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "school";
                        return RedirectToAction("GetAllSchool", "School");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAllSchool", "School");
                }
                ViewBag.ActiveMenu = "school";
                return RedirectToAction("GetAllSchool", "School");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAllSchool", "School");
            }
        }


        [HttpPost]
        public IActionResult DeleteMultipleSchools([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                foreach (var id in request.Ids)
                {
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "School/Delete/" + id, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to delete school with ID {id}");
                    }
                }
                ViewBag.ActiveMenu = "school";
                return RedirectToAction("GetAllSchool");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "school";
                return View();
            }
        }

        public class DeleteMultipleRequest
        {
            public List<int> Ids { get; set; }
        }
    }
}
