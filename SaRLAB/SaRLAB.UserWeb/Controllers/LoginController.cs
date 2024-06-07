using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SaRLAB.UserWeb.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SaRLAB.UserWeb.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;


        public LoginController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;
            _configuration["PathFolder:Value"] = "https://localhost:7050//";
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
                    Console.WriteLine($"Email: {claim.Value}");
                }
                else if (claim.Type == ClaimTypes.Role)
                {
                    Console.WriteLine($"Role: {claim.Value}");
                }
            }
        }

        //---------------------------------------login---------------
        [HttpGet]
        public IActionResult Login()
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
        public IActionResult Login(User login)
        {
            List<LoginDto> users = new List<LoginDto>();


            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password).Result;

            Console.WriteLine(_httpClient.BaseAddress + "Login/login/" + login.Email + "/" + login.Password);

            Console.WriteLine(response);

            if (response.IsSuccessStatusCode)
            {

                string jwtToken = response.Content.ReadAsStringAsync().Result;

                Program.jwtToken = jwtToken;

                DecodeJwtToken(jwtToken);

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.ReadJwtToken(jwtToken);

                foreach (Claim claim in token.Claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        return RedirectToAction("Index", "HomePage");
                    }
                }
                return View();
            }
            else
            {
                TempData["Error"] = "Không có tài khoản. Vui lòng thử lại!";
                return View();
            }
        }

        //----------------------------register-------------------------
        [HttpPost]
        public IActionResult Register(User user)
        {
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
                    return RedirectToAction("Login");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }
    }
}
