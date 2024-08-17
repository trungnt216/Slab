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

        private readonly IHttpContextAccessor _httpContextAccessor;


        UserDto userLogin = new UserDto();

        List<Subject> subjects = new List<Subject>();
        List<NoticeAdmin> notice = new List<NoticeAdmin>();

        List<ManageTitle> manageTitles = new List<ManageTitle>();

        public SchoolController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var httpContext = _httpContextAccessor.HttpContext;
            var jwtToken = httpContext.Session.GetString("jwtToken");




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

        //------------------------------------------------- school ------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult GetAllSchool()
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
        public ActionResult CreateSchool(School school, IFormFile BranchFile, IFormFile SchoolFile)
        {
            if (BranchFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(BranchFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    BranchFile.CopyTo(stream);
                }
                school.BranchSumary = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (SchoolFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(SchoolFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    SchoolFile.CopyTo(stream);
                }
                school.SchoolSumary = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

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
        public ActionResult EditSchool(School school, IFormFile BranchFile, IFormFile SchoolFile)
        {
            if (BranchFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(BranchFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    BranchFile.CopyTo(stream);
                }
                school.BranchSumary = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (SchoolFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(SchoolFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    SchoolFile.CopyTo(stream);
                }
                school.SchoolSumary = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

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
