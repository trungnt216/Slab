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

namespace SaRLAB.AdminWeb.Controllers
{
    public class ConfigurationController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;


        User userLogin = new User();

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
                    
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAllBanner()
        {
            List<Banner> banner = new List<Banner>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Banner/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                banner = JsonConvert.DeserializeObject<List<Banner>>(data);
            }

            return View(banner);
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            List<UserDto> users = new List<UserDto>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAll").Result;

            Console.WriteLine(_httpClient.BaseAddress + "User/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UserDto>>(data);
            }

            return View(users);

        }

        public IActionResult GetUsedByRole()
        {
            return View();
        }

        [HttpGet]
        public IActionResult InsertUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InsertUser(User user, IFormFile FileImage) {
            if(user == null) 
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
                user.CreateBy = userLogin.Email;
                user.UpdateBy = userLogin.Email;
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/register", content).Result;

                if(response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAllUser");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }


        [HttpGet]
        public IActionResult Edit(string email)
        {
            User user = new User();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(data);
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(User user, IFormFile FileImage)
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
                _user.AvtPath = filePath;
            }

            try
            {
                string data = JsonConvert.SerializeObject(_user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAllUser");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
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
                    return RedirectToAction("GetAllUser");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAllUser");
        }

        public IActionResult PostManagement()
        {
            return View();
        }

        [HttpGet]
        public IActionResult InsertBanner()
        {
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
                    banner.PathImage = "https://localhost:7135//uploads/" + uniqueFileName;
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
                return View();
            }

            return View();
        }

        [HttpGet]
        public IActionResult EditBanner(string id)
        {
            Banner banner = new Banner();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Banner/GetByID/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                banner = JsonConvert.DeserializeObject<Banner>(data);
            }
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

                _banner.PathImage = filePath;
            }

            try
            {
                string data = JsonConvert.SerializeObject(_banner);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Banner/Update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Banner update success";
                    return RedirectToAction("GetAllBanner");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
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
                    return RedirectToAction("GetAllBanner");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAllBanner");
        }

        [HttpPost]
        public IActionResult DeleteMultipleBanners([FromBody] DeleteMultipleRequest request)
        {
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
                return RedirectToAction("GetAllBanner");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
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
            User user = new User();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + userLogin.Email).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(data);
            }
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
                _user.AvtPath = filePath;
            }

            try
            {
                string data = JsonConvert.SerializeObject(_user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "User/update", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAllUser");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }
    }
}
