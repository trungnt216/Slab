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
        string pathFolderSave = "https://localhost:7135//uploads/";

        Uri baseAddress = new Uri("http://localhost:5200/api/");

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;


        UserDto userLogin = new UserDto();

        public SchoolController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env)
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
                if (claim.Type == "SchoolId")
                {
                    userLogin.SchoolId = int.Parse(claim.Value);
                }
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        }

        //------------------------------------------------- school ------------------------------------------------------------------------------

        [HttpGet]
        public IActionResult GetAllSchool()
        {
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
        public ActionResult CreateSchool(School school)
        {
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
        public ActionResult EditSchool(School school)
        {
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
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "School/Delete/" + id).Result;

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
                    HttpResponseMessage response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "School/Delete/" + id).Result;

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
