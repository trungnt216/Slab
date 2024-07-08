using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace SaRLAB.UserWeb.Controllers
{
    public class LibraryController : Controller
    {
        string pathFolderSave = null;

        private readonly IWebHostEnvironment _env;
        Uri baseAddress = new Uri(Program.api);
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        Subject subject1 = new Subject();
        Subject subject2 = new Subject();
        Subject subject3 = new Subject();
        Subject subject4 = new Subject();
        Subject subject5 = new Subject();
        Subject subject6 = new Subject();

        public LibraryController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            if(jwtToken == null){
                _hasError = true;
                return;
            }

            pathFolderSave = _configuration["PathFolder:Value"];

            Console.WriteLine(pathFolderSave);

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

            if (userLogin.RoleName == ("Admin"))
            {
                checkRole = 1;
            }

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

        //------------------------------- thư viện Library ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Library(int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            

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

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/"+ subjectID +"/LIBRARY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
                documents = documents.Where(doc => doc.PageFlag == true).ToList();
            }

            ViewData["subject"] = subjectID;

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(documents);
        }

        [HttpGet]
        public ActionResult Create_Library(int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                ViewData["subject"] = subjectID;
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Library", new { subjectID });
            }
        }
        [HttpPost]
        public ActionResult Create_Library(Document document, IFormFile File, int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;

            if (File != null && document.Path == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Document");

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
                document.Path = pathFolderSave + "FileFolder/Document/" + uniqueFileName;
            }

            try
            {
                document.CreateTime = DateTime.Now;
                document.CreateBy = userLogin.Email;
                document.UpdateTime = DateTime.Now;
                document.UpdateBy = userLogin.Email;
                document.SchoolId = userLogin.SchoolId;
                document.SubjectId = subjectID;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "LIBRARY";
                document.PageFlag = true;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Library", new {subjectID});
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View();
        }

        [HttpGet]
        public ActionResult Details_Library(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewData["subject"] = document.SubjectId;

            return View(document);
        }

        [HttpGet]
        public ActionResult Edit_Library(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewData["subject"] = document.SubjectId;

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Library", new { document.SubjectId });
            }
        }
        [HttpPost]
        public ActionResult Edit_Library(Document document, IFormFile File, int subjectID)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;

            if (File != null && document.Path == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Document");

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
                document.Path = pathFolderSave + "FileFolder/Document/" + uniqueFileName;
            }

            try
            {
                document.UpdateTime = DateTime.Now;
                document.UpdateBy = userLogin.Email;
                document.PageFlag = true;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    ViewData["subject"] = subjectID;
                    return RedirectToAction("GetAll_Library", new { subjectID });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View();
        }


        public ActionResult Delete_Library(int id, int subjectID)
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Library", new { subjectID });
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
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "experience";
                        return RedirectToAction("GetAll_Library", new { subjectID });
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Library", new { subjectID });
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Library", new { subjectID });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Library", new { subjectID });
            }
        }
    }
}
