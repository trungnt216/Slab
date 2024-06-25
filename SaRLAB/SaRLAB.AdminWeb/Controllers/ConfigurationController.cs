using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SaRLAB.AdminWeb.Controllers
{
    public class ConfigurationController : Controller
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

        public ConfigurationController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env)
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

        public IActionResult Index()
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

            return View();
        }


        //----------------------------------------------------------------------------------------------------------------------
        public ActionResult Recover_Banner()
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

            ViewBag.ActiveMenu = "banner";

            try
            {

                string data = JsonConvert.SerializeObject(school);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage responses = _httpClient.PostAsync(_httpClient.BaseAddress + "School/RecoverSchool/" + userLogin.SchoolId, content).Result;

                if (responses.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllBanner");
                }
            }
            catch (Exception ex)
            {
                RedirectToAction("GetAllBanner");
            }
            return RedirectToAction("GetAllBanner");
        }
        [HttpGet]
        public IActionResult GetAllBanner()
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

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetByID/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                school = JsonConvert.DeserializeObject<School>(data);
            }
            ViewBag.ActiveMenu = "banner";
            return View(school);
        }
        [HttpPost]
        public ActionResult GetAllBanner(School school,IFormFile FileChemLogo, IFormFile FileBioLogo, IFormFile FilePhysLogo, 
            IFormFile FileBiochemLogo, IFormFile FileBanner, IFormFile FileLogoSchool, IFormFile FileBackupSubject1Logo,
            IFormFile FileBackupSubject2Logo, IFormFile FileBackupSubject3Logo, IFormFile FileBackupSubject4Logo,
            IFormFile FileBackupSubject5Logo, IFormFile FileBackupSubject6Logo)
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

            if (FileChemLogo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileChemLogo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileChemLogo.CopyTo(stream);
                }
                school.ChemLogo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBioLogo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBioLogo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBioLogo.CopyTo(stream);
                }
                school.BioLogo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FilePhysLogo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FilePhysLogo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FilePhysLogo.CopyTo(stream);
                }
                school.PhysLogo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBiochemLogo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBiochemLogo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBiochemLogo.CopyTo(stream);
                }
                school.BiochemLogo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBanner != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBanner.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBanner.CopyTo(stream);
                }
                school.Banner = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileLogoSchool != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileLogoSchool.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileLogoSchool.CopyTo(stream);
                }
                school.SchoolLogo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBackupSubject1Logo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBackupSubject1Logo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBackupSubject1Logo.CopyTo(stream);
                }
                school.BackupSubject1Logo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBackupSubject2Logo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBackupSubject2Logo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBackupSubject2Logo.CopyTo(stream);
                }
                school.BackupSubject2Logo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBackupSubject3Logo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBackupSubject3Logo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBackupSubject3Logo.CopyTo(stream);
                }
                school.BackupSubject3Logo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBackupSubject4Logo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBackupSubject4Logo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBackupSubject4Logo.CopyTo(stream);
                }
                school.BackupSubject4Logo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBackupSubject5Logo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBackupSubject5Logo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBackupSubject5Logo.CopyTo(stream);
                }
                school.BackupSubject5Logo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            if (FileBackupSubject6Logo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/School");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileBackupSubject6Logo.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileBackupSubject6Logo.CopyTo(stream);
                }
                school.BackupSubject6Logo = pathFolderSave + "FileFolder/School/" + uniqueFileName;
            }

            try
            {
                string data = JsonConvert.SerializeObject(school);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "School/Update/" + userLogin.SchoolId, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return View();
        }
        //-------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult GetAllUser()
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

            List<UserDto> users = new List<UserDto>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAll").Result;

            Console.WriteLine(_httpClient.BaseAddress + "User/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserDto>>(data);
            }
            ViewBag.ActiveMenu = "user";
            return View(users);

        }

        [HttpGet]
        public IActionResult InsertUser()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<School> schools = new List<School>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "School/GetAllSchool").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                schools = JsonConvert.DeserializeObject<List<School>>(data);
            }
            // Tạo danh sách SelectListItem từ danh sách trường học
            List<SelectListItem> schoolItems = schools.Select(s => new SelectListItem
            {
                Value = s.ID.ToString(), // Giá trị của mỗi item là Id của trường học
                Text = s.Name // Hiển thị tên của trường học
            }).ToList();

            // Thêm một option mặc định cho người dùng chọn
            schoolItems.Insert(0, new SelectListItem { Value = "", Text = "Chọn trường học" });

            // Truyền danh sách SelectListItem vào ViewBag
            ViewBag.Schools = schoolItems;

            ViewBag.ActiveMenu = "user";

            return View();
        }
        [HttpPost]
        public IActionResult InsertUser(User user) {

            if (user == null) 
            {
                ViewBag.ActiveMenu = "user";
                return View();
            }

            try 
            {
                user.CreateBy = userLogin.Email;
                user.UpdateBy = userLogin.Email;
                user.Role_ID = 5;
                user.AvtPath = "~/images/defaultAvatar.jpg";
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/register", content).Result;

                if(response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    ViewBag.ActiveMenu = "user";
                    return RedirectToAction("GetAllUser");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "user";
            return View();
        }


        [HttpGet]
        public IActionResult Edit(string email)
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

            User user = new User();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(data);
            }

            if (user.Role_ID == 2)
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa";
                return RedirectToAction("GetAllUser");
            }

            ViewBag.ActiveMenu = "user";
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(User user, IFormFile FileImage)
        {
            user.CreateBy = userLogin.Email;
            user.CreateTime = DateTime.Now;
            user.UpdateBy = userLogin.Email;
            user.AvtPath = userLogin.AvtPath;

            if (FileImage != null)
            {
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "BannerImage");

                string uploadsFolder = Path.Combine(_env.WebRootPath, "image/avatar_user");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileImage.CopyTo(stream);
                }
                user.AvtPath = pathFolderSave + "image/avatar_user/" + uniqueFileName; ;
            }

            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    ViewBag.ActiveMenu = "user";
                    return RedirectToAction("GetAllUser");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "user";
            return View();
        }

        public IActionResult Delete(int id)
        {
            User user = new User();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID_ID/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(data);
            }


            if (user.Role_ID == 2)
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa";
                return RedirectToAction("GetAllUser");
            }

            try
            {
                Console.WriteLine(id.ToString());
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response1 = _httpClient.PostAsync(_httpClient.BaseAddress + "User/DeleteById/" + id,content).Result;

                Console.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ActiveMenu = "user";
                    return RedirectToAction("GetAllUser");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "user";
            return RedirectToAction("GetAllUser");
        }

        public IActionResult DeleteMultipleUsers([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                foreach (var id in request.Ids)
                {
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/DeleteById/" + id,content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to delete user with ID {id}");
                    }
                }
                ViewBag.ActiveMenu = "user";
                return RedirectToAction("GetAllUser");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "user";
                return View();
            }
        }


        [HttpGet]
        public IActionResult InsertBanner()
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


            ViewBag.ActiveMenu = "banner";
            return View();
        }
        [HttpPost]
        public IActionResult InsertBanner(IFormFile FileImage)
        {
            if (FileImage != null)
            {
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "BannerImage");

                string uploadsFolder = Path.Combine(_env.WebRootPath, "image/banner");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileImage.CopyTo(stream);
                }
                try
                {
                    Banner banner = new Banner();
                    banner.PathImage = pathFolderSave + "image/banner/" + uniqueFileName;
                    banner.CreateBy = userLogin.Email;
                    banner.CreateTime = DateTime.Now;
                    banner.UpdateTime = DateTime.Now;
                    banner.UpdateBy = userLogin.Email;


                    string data = JsonConvert.SerializeObject(banner);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Banner/Insert", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Status"] = "insert success";
                        ViewBag.ActiveMenu = "banner";
                        return RedirectToAction("GetAllBanner");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Status"] = $"{ex.Message}";
                    return View();
                }
            }
            else
            {
                ViewBag.ActiveMenu = "banner";
                return View();
            }
            ViewBag.ActiveMenu = "banner";
            return View();
        }

        [HttpGet]
        public IActionResult EditBanner(int id)
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
            /*            string substringToRemove = "/undefined";

                        if (id.EndsWith(substringToRemove))
                        {
                            // Remove the undesired substring
                            id = id.Remove(id.Length - substringToRemove.Length);
                        }*/

            Banner banner = new Banner();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Banner/GetByID/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                banner = JsonConvert.DeserializeObject<Banner>(data);
            }

            /*            banner.ID = Convert.ToInt32(id);*/
            ViewBag.ActiveMenu = "banner";
            return View(banner);
        }
        [HttpPost]
        public IActionResult EditBanner(Banner banner, IFormFile FileImage)
        {
            var _banner = new Banner();
            _banner.ID = banner.ID;
            _banner.CreateBy = banner.CreateBy;
            _banner.CreateTime = banner.CreateTime;
            _banner.UpdateTime = DateTime.Now;
            _banner.UpdateBy = userLogin.Email;
            _banner.PathImage = banner.PathImage;
            _banner.status = banner.status;

            if (FileImage != null)
            {
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "BannerImage");

                string uploadsFolder = Path.Combine(_env.WebRootPath, "image/banner");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    FileImage.CopyTo(stream)
;
                }

                _banner.PathImage = pathFolderSave + "image/banner/" + uniqueFileName; ;
            }

            try
            {
                string data = JsonConvert.SerializeObject(_banner);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Banner/Update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Banner update success";
                    ViewBag.ActiveMenu = "banner";
                    return RedirectToAction("GetAllBanner");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "banner";
            return View();
        }

        public IActionResult DeleteBanner(int id)
        {
            try
            {
                Console.WriteLine(id.ToString());
                HttpResponseMessage response;
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "Banner/DeleteById/" + id,content).Result;

                Console.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.ActiveMenu = "banner";
                    return RedirectToAction("GetAllBanner");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "banner";
                return View();
            }
            ViewBag.ActiveMenu = "banner";
            return RedirectToAction("GetAllBanner");
        }

        [HttpPost]
        public IActionResult DeleteMultipleBanners([FromBody] DeleteMultipleRequest request)
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

            try
            {
                foreach (var id in request.Ids)
                {
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Banner/DeleteById/" + id,content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Failed to delete banner with ID {id}");
                    }
                }
                ViewBag.ActiveMenu = "banner";
                return RedirectToAction("GetAllBanner");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "banner";
                return View();
            }
        }

        public class DeleteMultipleRequest
        {
            public List<int> Ids { get; set; }
        }


        //action fix the information of the user
        [HttpGet]
        public IActionResult Fix_Information()
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

            User user = new User();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + userLogin.Email).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(data);
            }
            ViewBag.ActiveMenu = "user";
            return View(user);
        }
        [HttpPost]
        public IActionResult Fix_Information(User user, IFormFile File)
        {
            user.CreateBy = user.CreateBy;
            user.CreateTime = user.CreateTime;
            user.UpdateBy = userLogin.Email;

            if (File != null)
            {

                string uploadsFolder = Path.Combine(_env.WebRootPath, "image/avatar_user");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }
                user.AvtPath = pathFolderSave + "image/avatar_user/" + uniqueFileName; ;
            }

            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("Fix_Information");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        //----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_User()
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

            List<UserDto> users = new List<UserDto>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllUserInSchoolRoleUser/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserDto>>(data);
            }

            ViewBag.ActiveMenu = "student";
            return View(users);

        }


        [HttpGet]
        public IActionResult Edit_User(string email)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;


            SubjectFlag user = new SubjectFlag();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "SubjectFlag/GetByID/" + email).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<SubjectFlag>(data);
            }

            ViewBag.ActiveMenu = "student";

            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            return View(user);
        }
        [HttpPost]
        public IActionResult Edit_User(SubjectFlag sub)
        {

            try
            {
                string data = JsonConvert.SerializeObject(sub);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "SubjectFlag/Update/" + sub.UserEmail, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    ViewBag.ActiveMenu = "student";
                    return RedirectToAction("GetAll_User");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            ViewBag.ActiveMenu = "student";
            return View();
        }
    }
}
