using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using NuGet.Protocol;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using SaRLAB.AdminWeb.Models;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SaRLAB.AdminWeb.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri(Program.api);

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;

        private readonly IHttpContextAccessor _httpContextAccessor;

        UserDto userLogin = new UserDto();
        List<NoticeAdmin> notice = new List<NoticeAdmin>();

        public LoginController(IConfiguration configuration, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;


            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var httpContext = _httpContextAccessor.HttpContext;
            var jwtToken = httpContext.Session.GetString("jwtToken");
        }

        public void DecodeJwtToken(string jwtToken)
        {
            // Create a JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Parse the JWT token
            var token = tokenHandler.ReadJwtToken(jwtToken);

            // Access the claims from the JWT token
            /*            foreach (Claim claim in token.Claims)
                        {
                            Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                        }*/

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
        }

        public IActionResult Index()
        {
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

            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginDto login)
        {

            List<LoginDto> users = new List<LoginDto>();


            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password).Result;

            Console.WriteLine(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password);

            Console.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {

                string jwtToken = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(jwtToken);

                /*Program.jwtToken = jwtToken;*/

                HttpContext.Session.SetString("jwtToken", jwtToken);

                DecodeJwtToken(jwtToken);

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.ReadJwtToken(jwtToken);

                foreach (Claim claim in token.Claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        Console.WriteLine(claim.Value);
                        if (!claim.Value.Equals("Admin") && !claim.Value.Equals("Owner"))
                        {
                            TempData["Error"] = "Tài khoản này không có quyền truy cập. Vui lòng thử lại!";
                            return View("Index");
                        }
                    }
                }

                StringContent content = new StringContent("", Encoding.UTF8, "application/json");

                HttpResponseMessage response2 = _httpClient.PostAsync(_httpClient.BaseAddress + "User/UpdateSchool/" + login.Email + "/" + login.SchoolId, content).Result;

                HttpResponseMessage response3;
                response3 = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password).Result;

                string jwtToken1 = response3.Content.ReadAsStringAsync().Result;

                /*Program.jwtToken = jwtToken1;*/
                HttpContext.Session.SetString("jwtToken", jwtToken1);

                /* return RedirectToAction("Index", "Home");*/
                return RedirectToAction("GetAllBanner", "Configuration");
            }
            else
            {
                TempData["Error"] = "Không có tài khoản. Vui lòng thử lại!";
                return View("Index");
            }
        }

        /*[HttpGet]
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

        }*/


        //create the action register
        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterUser(User user, IFormFile FileImage)
        {
            if (user == null)
            {
                return View();
            }

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
                user.AvtPath = filePath;
            }

            try
            {
                user.CreateBy = user.Email;
                user.UpdateBy = user.Email;
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/register", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Logout()
        {
            Program.jwtToken = null;
            return RedirectToAction("Index");
        }
    }
}
