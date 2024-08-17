using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Common;
using SaRLAB.AdminWeb.Models;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SaRLAB.AdminWeb.Controllers
{
    public class HomeController : Controller
    {
        Uri baseAddress = new Uri(Program.api);

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IHttpContextAccessor _httpContextAccessor;

        UserDto userLogin = new UserDto();

        List<Subject> subjects = new List<Subject>();
        List<NoticeAdmin> notice = new List<NoticeAdmin>();

        List<ManageTitle> manageTitles = new List<ManageTitle>();

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var httpContext = _httpContextAccessor.HttpContext;
            var jwtToken = httpContext.Session.GetString("jwtToken");
            _httpContextAccessor = httpContextAccessor;

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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllDocumentsBySchoolToAccept/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                notice = JsonConvert.DeserializeObject<List<NoticeAdmin>>(data);
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Subject> subjects = new List<Subject>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetAll").Result;

            Console.WriteLine(response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                subjects = JsonConvert.DeserializeObject<List<Subject>>(data);
            }

            TempData["subject_1"] = subjects.SingleOrDefault(item => item.ID == 1).SubjectName;

            return View(subjects);
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            List<User> users = new List<User>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAll").Result;

            Console.WriteLine(_httpClient.BaseAddress + "User/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            return View(users);

        }

        [HttpGet]
        public IActionResult GetAllNotice()
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subjectname = subjects.SingleOrDefault(item => item.Type == i);
                if (subjectname != null)
                {
                    TempData[$"subject_{i}"] = subjectname.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;

            List<NoticeAdmin> documents = new List<NoticeAdmin>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllDocumentsBySchoolToAccept/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<NoticeAdmin>>(data);
            }

            return View(documents);

        }

        [HttpGet]
        public ActionResult Details_Notice(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subjectname = subjects.SingleOrDefault(item => item.Type == i);
                if (subjectname != null)
                {
                    TempData[$"subject_{i}"] = subjectname.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;

            NoticeAdmin document = new NoticeAdmin();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetByIdDocumentsBySchoolToAccept/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<NoticeAdmin>(data);
            }
            return View(document);
        }

        public ActionResult Accept_Notice(int id)
        {
            Document document = new Document();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAllNotice");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.PageFlag = true;
                    document.UpdateTime = DateTime.Now;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetAllNotice");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAllNotice");
                }
                return RedirectToAction("GetAllNotice");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                return RedirectToAction("GetAllNotice");
            }
        }

        public ActionResult Delete_Notice(int id)
        {
            Document document = new Document();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                return RedirectToAction("GetAllNotice");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Delete/" + id, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetAllNotice");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAllNotice");
                }
                return RedirectToAction("GetAllNotice");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                return RedirectToAction("GetAllNotice");
            }
        }

    }
}
