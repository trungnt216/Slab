using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SaRLAB.AdminWeb.Controllers
{
    public class Subject_12 : Controller
    {
        string pathFolderSave = Program.FilePath;

        private readonly IWebHostEnvironment _env;

        private readonly IHttpContextAccessor _httpContextAccessor;


        Uri baseAddress = new Uri(Program.api);

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int Subject_id = 12;

        List<Subject> subjects = new List<Subject>();
        List<NoticeAdmin> notice = new List<NoticeAdmin>();

        List<ManageTitle> manageTitles = new List<ManageTitle>();

        public Subject_12(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
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
        //------------------------------ sửa tên logo icon ----------------------------------------
        [HttpGet]
        public IActionResult Configuration_Subject()
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


            Subject subject = new Subject();
            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByTypeSchool/" + Subject_id + "/" + userLogin.SchoolId).Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject = JsonConvert.DeserializeObject<Subject>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationsubject2";
            ViewBag.ActiveSubMenuLv2 = "configurationsubject2";
            return View(subject);
        }

        [HttpPost]
        public IActionResult Configuration_Subject(Subject subject_new, IFormFile File)
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

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Subject");

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
                subject_new.VideoBackGround = pathFolderSave + "FileFolder/Subject/" + uniqueFileName;
            }

            if (userLogin.RoleName == "Owner")
            {
                try
                {
                    subject_new.SchoolId = userLogin.SchoolId;
                    string data = JsonConvert.SerializeObject(subject_new);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Subject/update", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["successMessage"] = "create success";

                        return RedirectToAction("Configuration_Subject");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveSubMenuLv2 = "internationalLevel";

                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject2";
                    ViewBag.ActiveSubMenu = "configurationsubject2";
                    ViewBag.ActiveSubMenuLv2 = "configurationsubject2";
                    return View();
                }
            }
            else
            {

                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "configurationsubject2";
                ViewBag.ActiveSubMenuLv2 = "configurationsubject2";
                return View();
            }


            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationsubject2";
            ViewBag.ActiveSubMenuLv2 = "configurationsubject2";
            return View();
        }

        //------------------------------ sửa tên title các trường ----------------------------------------
        [HttpGet]
        public IActionResult Configuration_Title()
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

            List<ManageTitle> manageTitle = new List<ManageTitle>();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "ManageTitle/GetTitleBySchoolAnSubject/" + userLogin.SchoolId + "/" + Subject_id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                manageTitle = JsonConvert.DeserializeObject<List<ManageTitle>>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationtitle1";
            ViewBag.ActiveSubMenuLv2 = "titlesubject2";
            return View(manageTitle);
        }


        [HttpGet]
        public IActionResult Create_Title()
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

            var subjectTypes = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Giáo trình" },
            new SelectListItem { Value = "2", Text = "Thực hành" },
            new SelectListItem { Value = "3", Text = "Virtual Lab" },
            new SelectListItem { Value = "4", Text = "Nghiên cứu khoa học" },
            new SelectListItem { Value = "5", Text = "Tiếng Anh chuyên ngành" }
        };

            ViewBag.SubjectTypes = subjectTypes;

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationtitle1";
            ViewBag.ActiveSubMenuLv2 = "titlesubject2";
            return View();
        }

        [HttpPost]
        public IActionResult Create_Title(ManageTitle manageTitle)
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

            var subjectTypes = new List<SelectListItem>
        {
            new SelectListItem { Value = "1", Text = "Giáo trình" },
            new SelectListItem { Value = "2", Text = "Thực hành" },
            new SelectListItem { Value = "3", Text = "Virtual Lab" },
            new SelectListItem { Value = "4", Text = "Nghiên cứu khoa học" },
            new SelectListItem { Value = "5", Text = "Tiếng Anh chuyên ngành" }
        };

            ViewBag.SubjectTypes = subjectTypes;

            try
            {
                manageTitle.SchoolId = userLogin.SchoolId;
                manageTitle.SubjectId = Subject_id;
                string data = JsonConvert.SerializeObject(manageTitle);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "ManageTitle/Insert", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Configuration_Title");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveSubMenuLv2 = "internationalLevel";

                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "configurationsubject2";
                ViewBag.ActiveSubMenuLv2 = "titlesubject2";
                return View();
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationsubject2";
            ViewBag.ActiveSubMenuLv2 = "titlesubject2";
            return View();
        }

        //------------------------- thực nghiệm -------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Document(string titleDocument)
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/" + titleDocument).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "virtuallabsubject2";
            ViewBag.ActiveSubMenuLv2 = titleDocument;
            return View(documents);
        }

        public ActionResult Accept_Document(int id, string titleDocument)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "virtuallabsubject2";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.PageFlag = true;
                    document.UpdateTime = DateTime.Now;
                    document.UpdateBy = userLogin.Email;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject2";
                        ViewBag.ActiveSubMenu = "virtuallabsubject2";
                        ViewBag.ActiveSubMenuLv2 = titleDocument;
                        return RedirectToAction("GetAll_Document", new { titleDocument });
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject2";
                    ViewBag.ActiveSubMenu = "virtuallabsubject2";
                    ViewBag.ActiveSubMenuLv2 = titleDocument;
                    return RedirectToAction("GetAll_Document", new { titleDocument });
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "virtuallabsubject2";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "virtuallabsubject2";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
            }
        }

        public ActionResult Delete_Document(int id, string titleDocument)
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
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveSubMenu = "virtuallabsubject2";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
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
                        ViewBag.ActiveMenu = "subject2";
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveSubMenu = "virtuallabsubject2";
                        ViewBag.ActiveSubMenuLv2 = titleDocument;
                        return RedirectToAction("GetAll_Document", new { titleDocument });
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject2";
                    ViewBag.ActiveSubMenu = "virtuallabsubject2";
                    ViewBag.ActiveSubMenuLv2 = titleDocument;
                    return RedirectToAction("GetAll_Document", new { titleDocument });
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "virtuallabsubject2";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "virtuallabsubject2";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
            }
        }


        [HttpGet]
        public ActionResult Details_Document(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "virtuallabsubject2";
            ViewBag.ActiveSubMenuLv2 = document.Type;
            return View(document);
        }

        //--------------------- mutiple choice - ---------------------------------
        [HttpGet]
        public IActionResult GetAll_Question()
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/" + Subject_id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
            ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
            return View(equipment);
        }

        [HttpGet]
        public ActionResult Create_Question()
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                return RedirectToAction("GetAll_Question");
            }
        }
        [HttpPost]
        public ActionResult Create_Question(Quiz quiz, IFormFile QuestionFile, IFormFile OptionAFile, IFormFile OptionBFile, IFormFile OptionCFile, IFormFile OptionDFile)
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;
            if (QuestionFile != null && quiz.QuestionImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(QuestionFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    QuestionFile.CopyTo(stream);
                }
                quiz.QuestionImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionAFile != null && quiz.OptionAImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionAFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionAFile.CopyTo(stream);
                }
                quiz.OptionAImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionBFile != null && quiz.OptionBImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionBFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionBFile.CopyTo(stream);
                }
                quiz.OptionBImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionCFile != null && quiz.OptionCImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionCFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionCFile.CopyTo(stream);
                }
                quiz.OptionCImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionDFile != null && quiz.OptionDImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionDFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionDFile.CopyTo(stream);
                }
                quiz.OptionDImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            try
            {
                quiz.SchoolId = userLogin.SchoolId;
                quiz.SubjectId = Subject_id;

                string data = JsonConvert.SerializeObject(quiz);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["notice"] = "Thêm câu hỏi thành công";
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject2";
                    ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                    ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
            ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Question(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;
            Quiz quiz = new Quiz();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetQuizById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                quiz = JsonConvert.DeserializeObject<Quiz>(data);
            }

            if (quiz == null)
            {
                TempData["notice"] = "Không tìm thấy dữ liệu";
            }

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                return View(quiz);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                return RedirectToAction("GetAll_Question");
            }
        }
        [HttpPost]
        public ActionResult Edit_Question(Quiz quiz, IFormFile QuestionFile, IFormFile OptionAFile, IFormFile OptionBFile, IFormFile OptionCFile, IFormFile OptionDFile)
        {
            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;
            if (QuestionFile != null && quiz.QuestionImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(QuestionFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    QuestionFile.CopyTo(stream);
                }
                quiz.QuestionImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionAFile != null && quiz.OptionAImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionAFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionAFile.CopyTo(stream);
                }
                quiz.OptionAImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionBFile != null && quiz.OptionBImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionBFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionBFile.CopyTo(stream);
                }
                quiz.OptionBImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionCFile != null && quiz.OptionCImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionCFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionCFile.CopyTo(stream);
                }
                quiz.OptionCImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionDFile != null && quiz.OptionDImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionDFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionDFile.CopyTo(stream);
                }
                quiz.OptionDImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            try
            {
                quiz.SchoolId = userLogin.SchoolId;
                quiz.SubjectId = 3;

                string data = JsonConvert.SerializeObject(quiz);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Update/" + quiz.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "cauhoiantoansubject2";
            ViewBag.ActiveSubMenuLv2 = "cauhoisubject2";
            return View();
        }

        public ActionResult Delete_Question(int id)
        {
            try
            {
                HttpResponseMessage response;
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Delete/" + id, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("GetAll_Question");
            }
            return RedirectToAction("GetAll_Question");
        }

        //----------------------------- An toàn phòng thí nghiệm --------------------------------------
        [HttpGet]
        public ActionResult Details_RuleClassroom()
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

            Subject subject = new Subject();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByTypeSchool/" + Subject_id + "/" + userLogin.SchoolId).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                subject = JsonConvert.DeserializeObject<Subject>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationsubject2";
            ViewBag.ActiveSubMenuLv2 = "ruleClassroomSubject2";
            return View(subject);
        }

        [HttpGet]
        public ActionResult Update_RuleClassroom()
        {

            TempData["name"] = userLogin.Name;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["role"] = userLogin.RoleName;
            for (int i = 1; i <= 30; i++)
            {
                var subject = subjects.SingleOrDefault(item => item.ID == i);
                if (subject != null)
                {
                    TempData[$"subject_{i}"] = subject.SubjectName;
                }
            }
            TempData["noticeCount"] = notice.Count;
            ViewBag.MenuItems = manageTitles;

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationsubject2";
            ViewBag.ActiveSubMenuLv2 = "ruleClassroomSubject2";
            return View();
        }

        [HttpPost]
        public ActionResult Update_RuleClassroom(IFormFile File)
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

            Subject subject = new Subject();

            if (File != null)
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
                subject.Rule = pathFolderSave + "FileFolder/Document/" + uniqueFileName;
            }

            try
            {
                subject.Type = Subject_id;
                subject.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(subject);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Subject/update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject2";
                    ViewBag.ActiveSubMenu = "configurationsubject2";
                    ViewBag.ActiveSubMenuLv2 = "ruleClassroomSubject2";
                    return RedirectToAction("Details_RuleClassroom");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject2";
                ViewBag.ActiveSubMenu = "configurationsubject2";
                ViewBag.ActiveSubMenuLv2 = "ruleClassroomSubject2";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject2";
            ViewBag.ActiveSubMenu = "configurationsubject2";
            ViewBag.ActiveSubMenuLv2 = "ruleClassroomSubject2";
            return View();
        }
    }
}
