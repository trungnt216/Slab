﻿using Microsoft.AspNetCore.Mvc;
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
        string pathFolderSave = "https://localhost:7135//uploads/";

        Uri baseAddress = new Uri("http://localhost:5200/api/");

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

            string jwtToken = _configuration["JwtToken:Value"];


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
            if(user == null) 
            {
                ViewBag.ActiveMenu = "user";
                return View();
            }

            try 
            {
                user.CreateBy = userLogin.Email;
                user.UpdateBy = userLogin.Email;
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
            ViewBag.ActiveMenu = "user";
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(User user)
        {
            user.CreateBy = userLogin.Email;
            user.CreateTime = DateTime.Now;
            user.UpdateBy = userLogin.Email;
            user.AvtPath = userLogin.AvtPath;

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

/*        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        [HttpDelete]*/
        public IActionResult Delete(int id)
        {
            try
            {
                Console.WriteLine(id.ToString());
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "User/DeleteById/" + id).Result;

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
                    HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "User/DeleteById/" + id).Result;

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
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Banner/DeleteById/" + id).Result;

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
                    HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Banner/DeleteById/" + id).Result;

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
        public IActionResult Fix_Information(User user, IFormFile FileImage)
        {
            var _user = new User();
            _user.Email = user.Email;
            _user.Password = user.Password;
            _user.Name = user.Name;
            _user.DateOfBirth = user.DateOfBirth;
            _user.CreateBy = user.CreateBy;
            _user.CreateTime = user.CreateTime;
            _user.UpdateBy = userLogin.Email;
            _user.Role_ID = user.Role_ID;
            _user.AvtPath = user.AvtPath;

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
                _user.AvtPath = pathFolderSave + "image/avatar_user/" + uniqueFileName; ;
            }

            try
            {
                string data = JsonConvert.SerializeObject(_user);
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
                ViewBag.ActiveMenu = "user";
                return View();
            }
            ViewBag.ActiveMenu = "user";
            return View();
        }
    }
}
