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
    public class ChemistryController : Controller
    {
        string pathFolderSave = null;

        private readonly IWebHostEnvironment _env;

        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        public ChemistryController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = _configuration["JwtToken:Value"];

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

        }

        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------
        //-------------------------------hoá học--------------------------------------------------------

        //----------------------------hóa chất ------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Chemistry()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/CHEMISTRYE").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "chemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Edit_Chemistry(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
        }
        [HttpPost]
        public ActionResult Edit_Chemistry(Equipment equipment, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create_Chemistry()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_Chemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.SubjectId = 1;
                equipment.Type = "CHEMISTRYE";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "chemistry";
                        return RedirectToAction("GetAll_Chemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }

        }


        [HttpGet]
        public ActionResult Details_Chemistry(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "chemistry";
            return View(equipment);
        }

        //----------------------------dụng cụ-----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_ToolChemistry()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/TOOLCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_ToolChemistry()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_ToolChemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.SubjectId = 1;
                equipment.Type = "TOOLCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View();
        }


        [HttpGet]
        public ActionResult Edit_ToolChemistry(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }
        [HttpPost]
        public ActionResult Edit_ToolChemistry(Equipment equipment, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                        return RedirectToAction("GetAll_ToolChemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }


        [HttpGet]
        public ActionResult Details_ToolChemistry(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View(equipment);
        }


        //------------------------Thiết bị ----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_EquipmentChemistry()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/EQUIPMENTCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_EquipmentChemistry()
        {
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_EquipmentChemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.SubjectId = 1;
                equipment.Type = "EQUIPMENTCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View();
        }


        [HttpGet]
        public ActionResult Edit_EquipmentChemistry(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return Ok();
            }
        }
        [HttpPost]
        public ActionResult Edit_EquipmentChemistry(Equipment equipment, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                        return RedirectToAction("GetAll_EquipmentChemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
        }


        [HttpGet]
        public ActionResult Details_EquipmentChemistry(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
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
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/EXPERIMENT").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Experiment()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }
        [HttpPost]
        public ActionResult Create_Experiment(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.Type = "EXPERIMENT";
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
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

        [HttpGet]
        public ActionResult Edit_Experiment(int id)
        {
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
                return RedirectToAction("GetAll_Experiment");
            }
        }
        [HttpPost]
        public ActionResult Edit_Experiment(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "experience";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }


        [HttpGet]
        public ActionResult Details_Experiment(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View(document);
        }

        //-------------------------------------- đại cương (Conspectus) ---------------------------------------------

        [HttpGet]
        public IActionResult GetAll_Conspectus()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/CONSPECTUS").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Conspectus()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
        }
        [HttpPost]
        public ActionResult Create_Conspectus(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.Type = "CONSPECTUS";
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "conspectus";
                    return RedirectToAction("GetAll_Conspectus");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Conspectus(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
        }
        [HttpPost]
        public ActionResult Edit_Conspectus(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "conspectus";
                    return RedirectToAction("GetAll_Conspectus");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "conspectus";
                        return RedirectToAction("GetAll_Conspectus");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "conspectus";
                    return RedirectToAction("GetAll_Conspectus");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "conspectus";
                return RedirectToAction("GetAll_Conspectus");
            }
        }


        [HttpGet]
        public ActionResult Details_Conspectus(int id)
        {
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
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "conspectus";
            return View(document);
        }

        //----------------------------Inorganic - organic ---------------- vô cơ hữu cơ ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Inorganic_Organic()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> document1 = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/INORGANIC").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document1 = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            List<Document> document2 = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/ORGANIC").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                document2 = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            List<Document> documents = new List<Document>();

            documents.AddRange(document1);
            documents.AddRange(document2);

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Inorganic_Organic()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return RedirectToAction("GetAll_Inorganic_Organic");
            }
        }
        [HttpPost]
        public ActionResult Create_Inorganic_Organic(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                    return RedirectToAction("GetAll_Inorganic_Organic");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Inorganic_Organic(int id)
        {
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
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return RedirectToAction("GetAll_Inorganic_Organic");
            }
        }
        [HttpPost]
        public ActionResult Edit_Inorganic_Organic(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                    return RedirectToAction("GetAll_Inorganic_Organic");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
            return View();
        }

        public ActionResult Delete_Inorganic_Organic(int id)
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
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return RedirectToAction("GetAll_Inorganic_Organic");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                        return RedirectToAction("GetAll_Inorganic_Organic");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                    return RedirectToAction("GetAll_Inorganic_Organic");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return RedirectToAction("GetAll_Inorganic_Organic");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
                return RedirectToAction("GetAll_Inorganic_Organic");
            }
        }


        [HttpGet]
        public ActionResult Details_Inorganic_Organic(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "inorganicOrganic";
            return View(document);
        }

        //----------------------------Inorganic  ---------------- vô cơ ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Inorganic()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/INORGANIC").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "inorganic";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Inorganic()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return RedirectToAction("GetAll_Inorganic");
            }
        }
        [HttpPost]
        public ActionResult Create_Inorganic(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "INORGANIC";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "giaotrinh";
                    ViewBag.ActiveSubMenuLv2 = "inorganic";
                    return RedirectToAction("GetAll_Inorganic");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "inorganic";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Inorganic(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return RedirectToAction("GetAll_Inorganic");
            }
        }
        [HttpPost]
        public ActionResult Edit_Inorganic(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "giaotrinh";
                    ViewBag.ActiveSubMenuLv2 = "inorganic";
                    return RedirectToAction("GetAll_Inorganic");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "inorganic";
            return View();
        }

        public ActionResult Delete_Inorganic(int id)
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
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return RedirectToAction("GetAll_Inorganic");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "giaotrinh";
                        ViewBag.ActiveSubMenuLv2 = "inorganic";
                        return RedirectToAction("GetAll_Inorganic");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "giaotrinh";
                    ViewBag.ActiveSubMenuLv2 = "inorganic";
                    return RedirectToAction("GetAll_Inorganic");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return RedirectToAction("GetAll_Inorganic");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "inorganic";
                return RedirectToAction("GetAll_Inorganic");
            }
        }


        [HttpGet]
        public ActionResult Details_Inorganic(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "inorganic";
            return View(document);
        }

        //---------------------------- organic ---------------- hữu cơ ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Organic()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/ORGANIC").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "organic";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Organic()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return RedirectToAction("GetAll_Organic");
            }
        }
        [HttpPost]
        public ActionResult Create_Organic(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "ORGANIC";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "giaotrinh";
                    ViewBag.ActiveSubMenuLv2 = "organic";
                    return RedirectToAction("GetAll_Organic");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "organic";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Organic(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return RedirectToAction("GetAll_Organic");
            }
        }
        [HttpPost]
        public ActionResult Edit_Organic(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "giaotrinh";
                    ViewBag.ActiveSubMenuLv2 = "organic";
                    return RedirectToAction("GetAll_Organic");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "organic";
            return View();
        }

        public ActionResult Delete_Organic(int id)
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
                return RedirectToAction("GetAll_Organic");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "giaotrinh";
                        ViewBag.ActiveSubMenuLv2 = "organic";
                        return RedirectToAction("GetAll_Organic");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "giaotrinh";
                    ViewBag.ActiveSubMenuLv2 = "organic";
                    return RedirectToAction("GetAll_Organic");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                return RedirectToAction("GetAll_Organic");
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "giaotrinh";
                ViewBag.ActiveSubMenuLv2 = "organic";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_Organic");
            }
        }


        [HttpGet]
        public ActionResult Details_Organic(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "giaotrinh";
            ViewBag.ActiveSubMenuLv2 = "organic";
            return View(document);
        }

        //--------------------------------hoạt tính sinh học---- biological ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Biological()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/BIOLOGICAL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Biological()
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
        }
        [HttpPost]
        public ActionResult Create_Biological(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "BIOLOGICAL";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "biological";
                    return RedirectToAction("GetAll_Biological");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Biological(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
        }
        [HttpPost]
        public ActionResult Edit_Biological(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "biological";
                    return RedirectToAction("GetAll_Biological");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "biological";
                        return RedirectToAction("GetAll_Biological");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "biological";
                    return RedirectToAction("GetAll_Biological");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "biological";
                return RedirectToAction("GetAll_Biological");
            }
        }


        [HttpGet]
        public ActionResult Details_Biological(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "biological";
            return View(document);
        }

        //-------------------------------Từ Vựng ----- Vocabulary ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Vocabulary()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/VOCABULARY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Vocabulary()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }
        [HttpPost]
        public ActionResult Create_Vocabulary(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "VOCABULARY";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "vocabulary";
                    return RedirectToAction("GetAll_Vocabulary");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View();
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Vocabulary(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }
        [HttpPost]
        public ActionResult Edit_Vocabulary(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "vocabulary";
                    return RedirectToAction("GetAll_Vocabulary");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "tienganh";
                        ViewBag.ActiveSubMenuLv2 = "vocabulary";
                        return RedirectToAction("GetAll_Vocabulary");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "vocabulary";
                    return RedirectToAction("GetAll_Vocabulary");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "vocabulary";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }


        [HttpGet]
        public ActionResult Details_Vocabulary(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "vocabulary";
            return View(document);
        }

        //-------------------------------Bài tập ----- Exam ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Exam()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/EXAM").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Exam()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
        }
        [HttpPost]
        public ActionResult Create_Exam(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "EXAM";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Exam");
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
        public ActionResult Edit_Exam(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
        }
        [HttpPost]
        public ActionResult Edit_Exam(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Exam");
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "tienganh";
                        ViewBag.ActiveSubMenuLv2 = "exam";
                        return RedirectToAction("GetAll_Exam");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "exam";
                    return RedirectToAction("GetAll_Exam");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "exam";
                return RedirectToAction("GetAll_Exam");
            }
        }


        [HttpGet]
        public ActionResult Details_Exam(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "exam";
            return View(document);
        }

        //-------------------------------Bài tập song ngữ ----- Examenglish ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Examenglish()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/EXAMENG").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Examenglish()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }
        }
        [HttpPost]
        public ActionResult Create_Examenglish(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "EXAMENG";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "examenglish";
                    return RedirectToAction("GetAll_Examenglish");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Examenglish(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return Ok();
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }
        }
        [HttpPost]
        public ActionResult Edit_Examenglish(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "examenglish";
                    return RedirectToAction("GetAll_Examenglish");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "tienganh";
                        ViewBag.ActiveSubMenuLv2 = "examenglish";
                        return RedirectToAction("GetAll_Examenglish");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "tienganh";
                    ViewBag.ActiveSubMenuLv2 = "examenglish";
                    return RedirectToAction("GetAll_Examenglish");
                }
                return RedirectToAction("GetAll_Examenglish");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "tienganh";
                ViewBag.ActiveSubMenuLv2 = "examenglish";
                return RedirectToAction("GetAll_Examenglish");
            }
        }


        [HttpGet]
        public ActionResult Details_Examenglish(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "tienganh";
            ViewBag.ActiveSubMenuLv2 = "examenglish";
            return View(document);
        }

        //-------------------------------Đề tài cấp sở----- Department_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Department_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/DEPARTMENTLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Department_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
        }
        [HttpPost]
        public ActionResult Create_Department_level(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "DEPARTMENTLEVEL";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                    return RedirectToAction("GetAll_Department_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Department_level(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chinh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
        }
        [HttpPost]
        public ActionResult Edit_Department_level(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                    return RedirectToAction("GetAll_Department_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "nghiencuu";
                        ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                        return RedirectToAction("GetAll_Department_level");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAll_Department_level");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "departmentLevel";
                return RedirectToAction("GetAll_Department_level");
            }
        }


        [HttpGet]
        public ActionResult Details_Department_level(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "departmentLevel";
            return View(document);
        }

        //-------------------------------Đề tài cấp tỉnh----- Provincial_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Provincial_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/PROVONCIALLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Provincial_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }
        }
        [HttpPost]
        public ActionResult Create_Provincial_level(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "PROVONCIALLEVEL";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                    return RedirectToAction("GetAll_Provincial_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Provincial_level(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }
        }
        [HttpPost]
        public ActionResult Edit_Provincial_level(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                    return RedirectToAction("GetAll_Provincial_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "nghiencuu";
                        ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                        return RedirectToAction("GetAll_Provincial_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                    return RedirectToAction("GetAll_Provincial_level");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                return RedirectToAction("GetAll_Provincial_level");
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "provincialLevel";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_Provincial_level");
            }
        }


        [HttpGet]
        public ActionResult Details_Provincial_level(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "provincialLevel";
            return View(document);
        }


        //-------------------------------Đề tài cấp quốc gia----- National_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_National_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_National_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_National_level");
            }
        }
        [HttpPost]
        public ActionResult Create_National_level(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "NATIONALLEVER";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                    return RedirectToAction("GetAll_National_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_National_level(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return RedirectToAction("GetAll_National_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return RedirectToAction("GetAll_National_level");
            }
        }
        [HttpPost]
        public ActionResult Edit_National_level(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                    return RedirectToAction("GetAll_National_level");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                return RedirectToAction("GetAll_National_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "nghiencuu";
                        ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                        return RedirectToAction("GetAll_National_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "nghiencuu";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                    return RedirectToAction("GetAll_National_level");
                }
                return RedirectToAction("GetAll_National_level");
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "nghiencuu";
                ViewBag.ActiveSubMenuLv2 = "nationalLevel";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_National_level");
            }
        }


        [HttpGet]
        public ActionResult Details_National_level(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nghiencuu";
            ViewBag.ActiveSubMenuLv2 = "nationalLevel";
            return View(document);
        }

        //------------------------------- câu hỏi chuẩn bị ----- Preparation_questions ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Preparation_questions()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/PREPARATIONQUESTION").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Preparation_questions()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }
        [HttpPost]
        public ActionResult Create_Preparation_questions(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "PREPARATIONQUESTION";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Preparation_questions(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View(document);
            }
            else
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }
        [HttpPost]
        public ActionResult Edit_Preparation_questions(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    HttpResponseMessage response;
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "thuchanh";
                        ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                        return RedirectToAction("GetAll_Preparation_questions");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }


        [HttpGet]
        public ActionResult Details_Preparation_questions(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestions";
            return View(document);
        }


        //------------------------------- báo cáo thực hành ----- Practice_report------------------------------------

        [HttpGet]
        public IActionResult GetAll_Practice_report()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/1/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Practice_report()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                return View();
            }
            else
            {
                TempData["notice"] = "banj khoong cos quyeen them mowi";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
        }
        [HttpPost]
        public ActionResult Create_Practice_report(Document document, IFormFile File)
        {
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
                document.SubjectId = 1;
                document.SchoolId = userLogin.SchoolId;
                document.Type = "NATIONALLEVER";
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "practiceReport";
                    return RedirectToAction("GetAll_Practice_report");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Practice_report(int id)
        {
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
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
        }
        [HttpPost]
        public ActionResult Edit_Practice_report(Document document, IFormFile File)
        {
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
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "practiceReport";
                    return RedirectToAction("GetAll_Practice_report");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return View();
            }
            ViewBag.ActiveMenu = "chem";
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
                ViewBag.ActiveMenu = "chem";
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
                    response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Document/Delete/" + id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "chem";
                        ViewBag.ActiveSubMenu = "thuchanh";
                        ViewBag.ActiveSubMenuLv2 = "practiceReport";
                        return RedirectToAction("GetAll_Practice_report");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "chem";
                    ViewBag.ActiveSubMenu = "thuchanh";
                    ViewBag.ActiveSubMenuLv2 = "practiceReport";
                    return RedirectToAction("GetAll_Practice_report");
                }
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "chem";
                ViewBag.ActiveSubMenu = "thuchanh";
                ViewBag.ActiveSubMenuLv2 = "practiceReport";
                return RedirectToAction("GetAll_Practice_report");
            }
        }


        [HttpGet]
        public ActionResult Details_Practice_report(int id)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "thuchanh";
            ViewBag.ActiveSubMenuLv2 = "practiceReport";
            return View(document);
        }

        //---------------------------- Ban chủ nhiệm -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Directors()
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "directors";
            return View(users);
        }

        [HttpGet]
        public IActionResult Details_Directors(string email)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "directors";
            return View(users);
        }

        //---------------------------- Giảng viên -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Teacher()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllTeacherUser/" + userLogin.SchoolId + "/1").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "teacher";
            return View(users);
        }

        [HttpGet]
        public IActionResult Details_Teacher(string email)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "teacher";
            return View(users);
        }

        //---------------------------- Tổ kĩ thuật -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Technical()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllTechnicalUser/" + userLogin.SchoolId + "/1").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "technical";
            return View(users);
        }

        [HttpGet]
        public IActionResult Details_Technical(string email)
        {
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

            ViewBag.ActiveMenu = "chem";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "technical";
            return View(users);
        }

    }
}