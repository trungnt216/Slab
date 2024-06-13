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
        string pathFolderSave = "https://admin.sarlabeducation.com//uploads/";

        Uri baseAddress = new Uri("http://api.sarlabeducation.com/api/");

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;


        UserDto userLogin = new UserDto();

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
            
        }

        public IActionResult Index()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            return View();
        }

        [HttpGet]
        public IActionResult GetAllBanner()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Banner> banner = new List<Banner>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Banner/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                banner = JsonConvert.DeserializeObject<List<Banner>>(data);
            }
            ViewBag.ActiveMenu = "banner";
            return View(banner);
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

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

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/image/avatar_user");

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


            ViewBag.ActiveMenu = "banner";
            return View();
        }
        [HttpPost]
        public IActionResult InsertBanner(IFormFile FileImage)
        {
            if (FileImage != null)
            {
                //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "BannerImage");

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/image/banner");

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

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/image/banner");

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

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/image/avatar_user");

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
                    return RedirectToAction("GetAllUser");
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
