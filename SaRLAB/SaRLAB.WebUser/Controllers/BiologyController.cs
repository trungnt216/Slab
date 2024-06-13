﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace SaRLAB.UserWeb.Controllers
{
    public class BiologyController : Controller
    {
        string pathFolderSave = null;

        private readonly IWebHostEnvironment _env;

        Uri baseAddress = new Uri("http://api.sarlabeducation.com/api/"); 
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        public BiologyController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            if(jwtToken == null)
            {
                _hasError = true;
                return; // Early exit from constructor
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

            SubjectFlag subjectFlag = new SubjectFlag();

            HttpResponseMessage response_sub = _httpClient.GetAsync(_httpClient.BaseAddress + "SubjectFlag/GetByID/" + userLogin.Email).Result;

            if (response_sub.IsSuccessStatusCode)
            {
                string data = response_sub.Content.ReadAsStringAsync().Result;
                subjectFlag = JsonConvert.DeserializeObject<SubjectFlag>(data);
            }

            else
            {
                if (subjectFlag.ChemistryMarkFlag == false)
                {
                    _hasError = true;
                    return; // Early exit from constructor
                }
            }
        }
        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------
        //-------------------------------hoá học--------------------------------------------------------
        public ActionResult Index()
        {
            ViewBag.ActiveMenu = "homePage";
            return View();
        }

        //----------------------------hóa chất ------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Chemistry()
        {

            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/3/CHEMISTRYE").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "chemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Edit_Chemistry(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy dữ liệu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
        }
        [HttpPost]
        public ActionResult Edit_Chemistry(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

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
                equipment.ImagePath = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }

            try
            {
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Update/" + equipment.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create_Chemistry()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_Chemistry(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

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
                equipment.ImagePath = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = 3;
                equipment.Type = "CHEMISTRYE";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "chemistry";
            return View();
        }


        public ActionResult Delete_Chemistry(int id)
        {

            Equipment equipment = new Equipment();
            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy dữ liệu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id,content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "chemistry";
                        return RedirectToAction("GetAll_Chemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }

        }


        [HttpGet]
        public ActionResult Details_Chemistry(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "chemistry";
            return View(equipment);
        }

        //----------------------------dụng cụ-----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_ToolChemistry()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/3/TOOLCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_ToolChemistry()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_ToolChemistry(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

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
                equipment.ImagePath = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = 3;
                equipment.Type = "TOOLCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View();
        }


        [HttpGet]
        public ActionResult Edit_ToolChemistry(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy thiết bị";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }
        [HttpPost]
        public ActionResult Edit_ToolChemistry(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

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
                equipment.ImagePath = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }

            try
            {
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Update/" + equipment.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View();
        }

        public ActionResult Delete_ToolChemistry(int id)
        {

            Equipment equipment = new Equipment();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                        return RedirectToAction("GetAll_ToolChemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }


        [HttpGet]
        public ActionResult Details_ToolChemistry(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View(equipment);
        }


        //------------------------Thiết bị ----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_EquipmentChemistry()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/3/EQUIPMENTCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_EquipmentChemistry()
        {
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_EquipmentChemistry(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

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
                equipment.ImagePath = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = 3;
                equipment.Type = "EQUIPMENTCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View();
        }


        [HttpGet]
        public ActionResult Edit_EquipmentChemistry(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return Ok();
            }
        }
        [HttpPost]
        public ActionResult Edit_EquipmentChemistry(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (File != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

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
                equipment.ImagePath = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }

            try
            {
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Update/" + equipment.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View();
        }

        public ActionResult Delete_EquipmentChemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                        return RedirectToAction("GetAll_EquipmentChemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
        }


        [HttpGet]
        public ActionResult Details_EquipmentChemistry(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View(equipment);
        }

        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------

        //------------------------- thực nghiệm -------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Experiment()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/EXPERIMENT").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Experiment()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }
        [HttpPost]
        public ActionResult Create_Experiment(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

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
                document.SubjectId = 3;
                document.Type = "EXPERIMENT";
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Experiment(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }
        [HttpPost]
        public ActionResult Edit_Experiment(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View();
        }


        public ActionResult Delete_Experiment(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "experience";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }


        [HttpGet]
        public ActionResult Details_Experiment(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View(document);
        }

        //-------------------------------------- đại cương (Conspectus) ---------------------------------------------

        [HttpGet]
        public IActionResult GetAll_Conspectus()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/CONSPECTUS").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Conspectus()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
        }
        [HttpPost]
        public ActionResult Create_Conspectus(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

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
                document.SubjectId = 3;
                document.Type = "CONSPECTUS";
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "conspectus";
                    return RedirectToAction("GetAll_Conspectus");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Conspectus(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
        }
        [HttpPost]
        public ActionResult Edit_Conspectus(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "conspectus";
                    return RedirectToAction("GetAll_Conspectus");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View();
        }

        public ActionResult Delete_Conspectus(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Delete/" + id,content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "conspectus";
                        return RedirectToAction("GetAll_Conspectus");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "conspectus";
                    return RedirectToAction("GetAll_Conspectus");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
        }


        [HttpGet]
        public ActionResult Details_Conspectus(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View(document);
        }


        //--------------------------------hoạt tính sinh học---- biological ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Biological()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/BIOLOGICAL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Biological()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
        }
        [HttpPost]
        public ActionResult Create_Biological(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "BIOLOGICAL";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "biological";
                    return RedirectToAction("GetAll_Biological");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Biological(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
        }
        [HttpPost]
        public ActionResult Edit_Biological(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "biological";
                    return RedirectToAction("GetAll_Biological");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View();
        }

        public ActionResult Delete_Biological(int id)
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
                return RedirectToAction("GetAll_Biological");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "biological";
                        return RedirectToAction("GetAll_Biological");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "biological";
                    return RedirectToAction("GetAll_Biological");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
        }


        [HttpGet]
        public ActionResult Details_Biological(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View(document);
        }

        //-------------------------------Từ Vựng ----- Vocabulary ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Vocabulary()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/VOCABULARY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Vocabulary()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }
        [HttpPost]
        public ActionResult Create_Vocabulary(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "VOCABULARY";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "vocabulary";
                    return RedirectToAction("GetAll_Vocabulary");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View();
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Vocabulary(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }
        [HttpPost]
        public ActionResult Edit_Vocabulary(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "vocabulary";
                    return RedirectToAction("GetAll_Vocabulary");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View();
        }

        public ActionResult Delete_Vocabulary(int id)
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
                return RedirectToAction("GetAll_Vocabulary");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "tienganh";
                        ViewBag.ActiveSubMenuLv2 = "vocabulary";
                        return RedirectToAction("GetAll_Vocabulary");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "vocabulary";
                    return RedirectToAction("GetAll_Vocabulary");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }


        [HttpGet]
        public ActionResult Details_Vocabulary(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View(document);
        }

        //-------------------------------Bài tập ----- Exam ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Exam()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/EXAM").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Exam()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
        }
        [HttpPost]
        public ActionResult Create_Exam(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "EXAM";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Exam");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Exam(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
        }
        [HttpPost]
        public ActionResult Edit_Exam(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Exam");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View();
        }

        public ActionResult Delete_Exam(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "tienganh";
                        ViewBag.ActiveSubMenuLv2 = "exam";
                        return RedirectToAction("GetAll_Exam");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Exam");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
        }


        [HttpGet]
        public ActionResult Details_Exam(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View(document);
        }

        //-------------------------------Bài tập song ngữ ----- Examenglish ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Examenglish()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/EXAMENG").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Examenglish()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }
        }
        [HttpPost]
        public ActionResult Create_Examenglish(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "EXAMENG";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "examenglish";
                    return RedirectToAction("GetAll_Examenglish");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Examenglish(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }
        }
        [HttpPost]
        public ActionResult Edit_Examenglish(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "examenglish";
                    return RedirectToAction("GetAll_Examenglish");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View();
        }

        public ActionResult Delete_Examenglish(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Delete/" + id,content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "tienganh";
                        ViewBag.ActiveSubMenuLv2 = "examenglish";
                        return RedirectToAction("GetAll_Examenglish");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "examenglish";
                    return RedirectToAction("GetAll_Examenglish");
                }
                return RedirectToAction("GetAll_Examenglish");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }
        }


        [HttpGet]
        public ActionResult Details_Examenglish(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View(document);
        }

        //-------------------------------Đề tài cấp sở----- Department_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Department_level()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/DEPARTMENTLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Department_level()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
        }
        [HttpPost]
        public ActionResult Create_Department_level(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "DEPARTMENTLEVEL";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                    return RedirectToAction("GetAll_Department_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Department_level(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chinh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
        }
        [HttpPost]
        public ActionResult Edit_Department_level(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                    return RedirectToAction("GetAll_Department_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View();
        }

        public ActionResult Delete_Department_level(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "nghiencuu";
                        ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                        return RedirectToAction("GetAll_Department_level");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAll_Department_level");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
        }


        [HttpGet]
        public ActionResult Details_Department_level(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(document);
        }

        //-------------------------------Đề tài cấp tỉnh----- Provincial_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Provincial_level()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/PROVONCIALLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Provincial_level()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }
        }
        [HttpPost]
        public ActionResult Create_Provincial_level(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "PROVONCIALLEVEL";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                    return RedirectToAction("GetAll_Provincial_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Provincial_level(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }
        }
        [HttpPost]
        public ActionResult Edit_Provincial_level(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                    return RedirectToAction("GetAll_Provincial_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View();
        }

        public ActionResult Delete_Provincial_level(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "nghiencuu";
                        ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                        return RedirectToAction("GetAll_Provincial_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                    return RedirectToAction("GetAll_Provincial_level");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_Provincial_level");
            }
        }


        [HttpGet]
        public ActionResult Details_Provincial_level(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View(document);
        }


        //-------------------------------Đề tài cấp quốc gia----- National_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_National_level()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_National_level()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_National_level");
            }
        }
        [HttpPost]
        public ActionResult Create_National_level(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "NATIONALLEVER";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                    return RedirectToAction("GetAll_National_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_National_level(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return RedirectToAction("GetAll_National_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return RedirectToAction("GetAll_National_level");
            }
        }
        [HttpPost]
        public ActionResult Edit_National_level(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                    return RedirectToAction("GetAll_National_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View();
        }

        public ActionResult Delete_National_level(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return RedirectToAction("GetAll_National_level");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "nghiencuu";
                        ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                        return RedirectToAction("GetAll_National_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                    return RedirectToAction("GetAll_National_level");
                }
                return RedirectToAction("GetAll_National_level");
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_National_level");
            }
        }


        [HttpGet]
        public ActionResult Details_National_level(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View(document);
        }

        //------------------------------- câu hỏi chuẩn bị ----- Preparation_questions ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Preparation_questions()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/PREPARATIONQUESTION").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Preparation_questions()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }
        [HttpPost]
        public ActionResult Create_Preparation_questions(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "PREPARATIONQUESTION";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Preparation_questions(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View(document);
            }
            else
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }
        [HttpPost]
        public ActionResult Edit_Preparation_questions(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View();
        }

        public ActionResult Delete_Preparation_questions(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "thuchanh";
                        ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                        return RedirectToAction("GetAll_Preparation_questions");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }


        [HttpGet]
        public ActionResult Details_Preparation_questions(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View(document);
        }


        //------------------------------- báo cáo thực hành ----- Practice_report------------------------------------

        [HttpGet]
        public IActionResult GetAll_Practice_report()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/3/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Practice_report()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
        }
        [HttpPost]
        public ActionResult Create_Practice_report(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.SubjectId = 3;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "NATIONALLEVER";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "practiceReport";
                    return RedirectToAction("GetAll_Practice_report");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Practice_report(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            if (document == null)
            {
                TempData["notice"] = "khong tim thay du lieu";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
        }
        [HttpPost]
        public ActionResult Edit_Practice_report(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "practiceReport";
                    return RedirectToAction("GetAll_Practice_report");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return View();
            }
            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View();
        }

        public ActionResult Delete_Practice_report(int id)
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
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice_report");
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
                        ViewBag.ActiveMenu = "bio";
                        ViewBag.ActiveSubMenu = "thuchanh";
                        ViewBag.ActiveSubMenuLv2 = "practiceReport";
                        return RedirectToAction("GetAll_Practice_report");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "bio";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "practiceReport";
                    return RedirectToAction("GetAll_Practice_report");
                }
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "bio";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
        }


        [HttpGet]
        public ActionResult Details_Practice_report(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View(document);
        }

        //---------------------------- Ban chủ nhiệm -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Directors()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllAdminUser/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "directors";
            return View(users);
        }

        [HttpGet]
        public IActionResult Details_Directors(string email)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "directors";
            return View(users);
        }

        //---------------------------- Giảng viên -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Teacher()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllTeacherUser/" + userLogin.SchoolId + "/3").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "teacher";
            return View(users);
        }

        [HttpGet]
        public IActionResult Details_Teacher(string email)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "teacher";
            return View(users);
        }

        //---------------------------- Tổ kĩ thuật -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Technical()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllTechnicalUser/" + userLogin.SchoolId + "/3").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "technical";
            return View(users);
        }

        [HttpGet]
        public IActionResult Details_Technical(string email)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "bio";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "technical";
            return View(users);
        }

    }
}