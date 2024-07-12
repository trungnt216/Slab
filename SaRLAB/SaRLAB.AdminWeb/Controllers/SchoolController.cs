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

        List<Subject> subjects = new List<Subject>();
        List<NoticeAdmin> notice = new List<NoticeAdmin>();

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

            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetAll").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subjects = JsonConvert.DeserializeObject<List<Subject>>(data);
            }

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllDocumentsBySchoolToAccept/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                notice = JsonConvert.DeserializeObject<List<NoticeAdmin>>(data);
            }

        }

        //------------------------------------------------- school ------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult GetAllSchool()
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath;TempData["role"] = userLogin.RoleName;
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
TempData["noticeCount"] = notice.Count;

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
            TempData["AvtPath"] = userLogin.AvtPath;TempData["role"] = userLogin.RoleName;
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
TempData["noticeCount"] = notice.Count;

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
            TempData["AvtPath"] = userLogin.AvtPath;TempData["role"] = userLogin.RoleName;
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
TempData["noticeCount"] = notice.Count;

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
