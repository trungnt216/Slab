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
    public class Subject_TestController : Controller
    {
        string pathFolderSave = null;

        int Subject_id = 23;

        private readonly IWebHostEnvironment _env;

        Uri baseAddress = new Uri(Program.api);
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        Subject subject = new Subject();

        Subject subject1 = new Subject();

        public Subject_TestController(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            if (jwtToken == null)
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
                if (subjectFlag.BiologyPermissionFlag == false)
                {
                    _hasError = true;
                    return; // Early exit from constructor
                }
            }

            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/" + Subject_id).Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }

        }
        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------
        //-------------------------------hoá học--------------------------------------------------------
        public ActionResult Index()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            ViewBag.ActiveMenu = "homePage";


            return View();
        }

        //----------------------------hóa chất ------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Chemistry()
        {

            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; TempData["subject_1"] = subject1.SubjectName;



            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/" + Subject_id + "/CHEMISTRYE").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_Chemistry(Equipment equipment, IFormFile File, IFormFile coverImage)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                equipment.CoverImage = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }
            else
            {
                equipment.CoverImage = "~/images/book.jpg";
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = Subject_id;
                equipment.Type = "CHEMISTRYE";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
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
                ViewBag.ActiveMenu = "subject6";
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
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "chemistry";
                        return RedirectToAction("GetAll_Chemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "chemistry";
                    return RedirectToAction("GetAll_Chemistry");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "chemistry";
                return RedirectToAction("GetAll_Chemistry");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "chemistry";
            return View(equipment);
        }

        //----------------------------dụng cụ-----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_ToolChemistry()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/" + Subject_id + "/TOOLCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_ToolChemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_ToolChemistry(Equipment equipment, IFormFile File, IFormFile coverImage)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                equipment.CoverImage = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }
            else
            {
                equipment.CoverImage = "~/images/book.jpg";
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = Subject_id;
                equipment.Type = "TOOLCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
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
                ViewBag.ActiveMenu = "subject6";
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
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                        return RedirectToAction("GetAll_ToolChemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "toolChemistry";
                return RedirectToAction("GetAll_ToolChemistry");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "toolChemistry";
            return View(equipment);
        }


        //------------------------Thiết bị ----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_EquipmentChemistry()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/" + Subject_id + "/EQUIPMENTCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_EquipmentChemistry()
        {
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
        }
        [HttpPost]
        public ActionResult Create_EquipmentChemistry(Equipment equipment, IFormFile File, IFormFile coverImage)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                equipment.CoverImage = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }
            else
            {
                equipment.CoverImage = "~/images/book.jpg";
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = Subject_id;
                equipment.Type = "EQUIPMENTCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
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
                ViewBag.ActiveMenu = "subject6";
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
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                        return RedirectToAction("GetAll_EquipmentChemistry");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                    return RedirectToAction("GetAll_EquipmentChemistry");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
                return RedirectToAction("GetAll_EquipmentChemistry");
            }
            else
            {
                ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistry";
            return View(equipment);
        }
        

        //---------------------------- Ban chủ nhiệm -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Directors()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllAdminUser/" + userLogin.SchoolId).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "directors";
            return View(users);
        }

        //---------------------------- Giảng viên -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Teacher()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllTeacherUser/" + userLogin.SchoolId + "/Subject_id").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "teacher";
            return View(users);
        }

        //---------------------------- Tổ kĩ thuật -----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Technical()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            List<User> users = new List<User>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetAllTechnicalUser/" + userLogin.SchoolId + "/Subject_id").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
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
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            User users = new User();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "User/GetByID/" + email).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<User>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nhansu";
            ViewBag.ActiveSubMenuLv2 = "technical";
            return View(users);
        }



        //----------------------------dụng cụ kho-----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_ToolChemistryStorage()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/" + Subject_id + "/TOOLCHEMISTRYSTORE").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_ToolChemistryStorage()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return View();
            }
            else
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                return RedirectToAction("GetAll_ToolChemistryStorage");
            }
        }
        [HttpPost]
        public ActionResult Create_ToolChemistryStorage(Equipment equipment, IFormFile File, IFormFile coverImage)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                equipment.CoverImage = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }
            else
            {
                equipment.CoverImage = "~/images/book.jpg";
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = Subject_id;
                equipment.Type = "TOOLCHEMISTRYSTORE";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "kho";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                    return RedirectToAction("GetAll_ToolChemistryStorage");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
            return View();
        }


        [HttpGet]
        public ActionResult Edit_ToolChemistryStorage(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return RedirectToAction("GetAll_ToolChemistryStorage");
            }
        }
        [HttpPost]
        public ActionResult Edit_ToolChemistryStorage(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "kho";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                    return RedirectToAction("GetAll_ToolChemistryStorage");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
            return View();
        }

        public ActionResult Delete_ToolChemistryStorage(int id)
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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return RedirectToAction("GetAll_ToolChemistryStorage");
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
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "kho";
                        ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                        return RedirectToAction("GetAll_ToolChemistryStorage");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "kho";
                    ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                    return RedirectToAction("GetAll_ToolChemistryStorage");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return RedirectToAction("GetAll_ToolChemistryStorage");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
                return RedirectToAction("GetAll_ToolChemistryStorage");
            }
        }


        [HttpGet]
        public ActionResult Details_ToolChemistryStorage(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "toolChemistryStorage";
            return View(equipment);
        }


        //------------------------Thiết bị kho----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_EquipmentChemistryStorage()
        {
            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/" + Subject_id + "/EQUIPMENTCHEMISTRYSTORE").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_EquipmentChemistryStorage()
        {
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return RedirectToAction("GetAll_EquipmentChemistryStorage");
            }
        }
        [HttpPost]
        public ActionResult Create_EquipmentChemistryStorage(Equipment equipment, IFormFile File, IFormFile coverImage)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Equipment");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                equipment.CoverImage = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;
            }
            else
            {
                equipment.CoverImage = "~/images/book.jpg";
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = Subject_id;
                equipment.Type = "EQUIPMENTCHEMISTRYSTORE";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "kho";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                    return RedirectToAction("GetAll_EquipmentChemistryStorage");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
            return View();
        }


        [HttpGet]
        public ActionResult Edit_EquipmentChemistryStorage(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            if (equipment == null)
            {
                TempData["notice"] = "không tìm thấy";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return Ok();
            }

            if (userLogin.Email == equipment.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return Ok();
            }
        }
        [HttpPost]
        public ActionResult Edit_EquipmentChemistryStorage(Equipment equipment, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "kho";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                    return RedirectToAction("GetAll_EquipmentChemistryStorage");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
            return View();
        }

        public ActionResult Delete_EquipmentChemistryStorage(int id)
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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return RedirectToAction("GetAll_EquipmentChemistryStorage");
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
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "kho";
                        ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                        return RedirectToAction("GetAll_EquipmentChemistryStorage");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "kho";
                    ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                    return RedirectToAction("GetAll_EquipmentChemistryStorage");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                return RedirectToAction("GetAll_EquipmentChemistryStorage");
            }
            else
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "kho";
                ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
                TempData["notice"] = "Bạn không có quyền xóa!";
                return RedirectToAction("GetAll_EquipmentChemistryStorage");
            }
        }


        [HttpGet]
        public ActionResult Details_EquipmentChemistryStorage(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "kho";
            ViewBag.ActiveSubMenuLv2 = "equipmentChemistryStorage";
            return View(equipment);
        }


        //----------------------------- Tin tức ------------------------------------------

        [HttpGet]
        public IActionResult GetAll_New()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/" + Subject_id + "/NEW").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "vipractice";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_New()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return RedirectToAction("GetAll_New");
            }
        }
        [HttpPost]
        public ActionResult Create_New(Document document, IFormFile File, IFormFile coverImage)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Document");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                document.CoverImage = pathFolderSave + "FileFolder/Document/" + uniqueFileName;
            }
            else
            {
                document.CoverImage = "~/images/book.jpg";
            }

            try
            {
                document.CreateTime = DateTime.Now;
                document.CreateBy = userLogin.Email;
                document.UpdateTime = DateTime.Now;
                document.UpdateBy = userLogin.Email;
                document.SchoolId = userLogin.SchoolId;
                document.SubjectId = Subject_id;
                document.Type = "NEW";
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = true;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "vipractice";
                    return RedirectToAction("GetAll_New");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "vipractice";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_New(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return RedirectToAction("GetAll_New");
            }
        }
        [HttpPost]
        public ActionResult Edit_New(Document document, IFormFile File)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "vipractice";
                    return RedirectToAction("GetAll_New");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "vipractice";
            return View();
        }


        public ActionResult Delete_New(int id)
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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return RedirectToAction("GetAll_New");
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
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "vipractice";
                        return RedirectToAction("GetAll_New");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "vipractice";
                    return RedirectToAction("GetAll_ViPractice");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return RedirectToAction("GetAll_New");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "vipractice";
                return RedirectToAction("GetAll_New");
            }
        }


        [HttpGet]
        public ActionResult Details_New(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "vipractice";
            return View(document);
        }

        //------------------------- document biến động -------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Document(string titleDocument)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; 
            TempData["subject_1"] = subject1.SubjectName;


            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/" + Subject_id + "/" + titleDocument).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View(documents);
        }


        [HttpGet]
        public ActionResult Create_Document(string titleDocument)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }
        [HttpPost]
        public ActionResult Create_Document(Document document, IFormFile File, IFormFile coverImage, string titleDocument)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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

            if (coverImage != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Document");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(coverImage.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    coverImage.CopyTo(stream);
                }
                document.CoverImage = pathFolderSave + "FileFolder/Document/" + uniqueFileName;
            }
            else
            {
                document.CoverImage = "~/images/book.jpg";
            }

            try
            {
                document.CreateTime = DateTime.Now;
                document.CreateBy = userLogin.Email;
                document.UpdateTime = DateTime.Now;
                document.UpdateBy = userLogin.Email;
                document.SchoolId = userLogin.SchoolId;
                document.SubjectId = Subject_id;
                document.Type = titleDocument;
                document.SchoolId = userLogin.SchoolId;
                document.PageFlag = false;

                string data = JsonConvert.SerializeObject(document);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Document(int id, string titleDocument)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }
        [HttpPost]
        public ActionResult Edit_Document(Document document, IFormFile File, string titleDocument)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


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
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View();
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
                ViewBag.ActiveMenu = "subject6";
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
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtualLab";
                        ViewBag.ActiveSubMenuLv2 = "experience";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtualLab";
                    ViewBag.ActiveSubMenuLv2 = "experience";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtualLab";
                ViewBag.ActiveSubMenuLv2 = "experience";
                return RedirectToAction("GetAll_Experiment");
            }
        }


        [HttpGet]
        public ActionResult Details_Document(int id, string titleDocument)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;


            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "experience";
            return View(document);
        }



        //----------------------------- An toàn phòng thí nghiệm --------------------------------------
        [HttpGet]
        public ActionResult Details_RuleClassroom(int id)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName;

            Subject subject = new Subject();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/" + Subject_id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                subject = JsonConvert.DeserializeObject<Subject>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtualLab";
            ViewBag.ActiveSubMenuLv2 = "vipractice";
            return View(subject);
        }
    }
}