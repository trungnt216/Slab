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
    public class Subject_30Controller : Controller
    {
        string pathFolderSave = null;

        int Subject_id = 30;

        string Subject_name = "~/Views/Subject_30/_Layout.cshtml";

        private readonly IWebHostEnvironment _env;

        Uri baseAddress = new Uri(Program.api);
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int checkRole = 0;

        private readonly bool _hasError = false;

        Subject subject = new Subject();

        Subject subject1 = new Subject();

        List<ManageTitle> manageTitles = new List<ManageTitle>();

        public Subject_30Controller(ILogger<HomePageController> logger, IConfiguration configuration, IWebHostEnvironment env)
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

            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetSubjectBySchoolAndType/" + userLogin.SchoolId + "/" + Subject_id).Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }

            HttpResponseMessage response_title = _httpClient.GetAsync(_httpClient.BaseAddress + "ManageTitle/GetTitleBySchoolAnSubject/" + userLogin.SchoolId + "/" + Subject_id).Result;
            if (response_title.IsSuccessStatusCode)
            {
                string data = response_title.Content.ReadAsStringAsync().Result;
                manageTitles = JsonConvert.DeserializeObject<List<ManageTitle>>(data);
            }

        }
        //----------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------
        //-------------------------------hoá học--------------------------------------------------------
        public ActionResult Index()
        {
            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

            ViewBag.ActiveMenu = "homePage";


            return View();
        }
        //-----------------kho ----------------------

        [HttpGet]
        public IActionResult GetAll_WareHouse(int id, string type)
        {

            if (_hasError || userLogin.RoleName == "User" || userLogin.RoleName == "Teacher")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            ViewBag.Layout = Subject_name;



            List<WareHouse> wareHouses = new List<WareHouse>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "WareHouseControler/GetWareHouseByEquipmentId/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                wareHouses = JsonConvert.DeserializeObject<List<WareHouse>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = type;
            ViewBag.idEquipment = id;
            return View(wareHouses);
        }

        [HttpGet]
        public ActionResult Create_WareHouse(int id, string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

            if (userLogin.RoleName == "Technical" || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                ViewBag.idEquipment = id;
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_WareHouse", new { id, type });
            }
        }
        [HttpPost]
        public ActionResult Create_WareHouse(WareHouse wareHouse, int id , string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

    
            try
            {
                wareHouse.ID = null;
                wareHouse.EquipmentId = id;
                wareHouse.AmountEquipment = 0;
                string data = JsonConvert.SerializeObject(wareHouse);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "WareHouseControler/Insert", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = type;
                    return RedirectToAction("GetAll_WareHouse", new { id, type });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return View();
            }
            return View();
        }

        public ActionResult Edit_WareHouse(int id, string type, Double amountEquipment, int idEquipment)
        {

            WareHouse wareHouse = new WareHouse();

            try
            {
                wareHouse.AmountEquipment = amountEquipment;
                wareHouse.ID = idEquipment;
                wareHouse.EquipmentId = id;

                string data = JsonConvert.SerializeObject(wareHouse);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "WareHouseControler/Update/" + idEquipment, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    List<WareHouse> wareHouses = new List<WareHouse>();

                    HttpResponseMessage response3 = _httpClient.GetAsync(_httpClient.BaseAddress + "WareHouseControler/GetWareHouseByEquipmentId/" + id).Result;

                    if (response3.IsSuccessStatusCode)
                    {
                        string data2 = response3.Content.ReadAsStringAsync().Result;
                        wareHouses = JsonConvert.DeserializeObject<List<WareHouse>>(data2);

                        Double totalAmountEquipment = (Double)wareHouses.Sum(wh => wh.AmountEquipment);

                        StringContent content1 = new StringContent("", Encoding.UTF8, "application/json");
                        HttpResponseMessage response1 = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/UpdateUnitEquipmentById/" + id + "/" + totalAmountEquipment, content).Result;
                    }

                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = type;
                    return RedirectToAction("GetAll_WareHouse", new { id, type });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_WareHouse", new { id, type });
            }
            return RedirectToAction("GetAll_WareHouse", new { id, type });
        }


            //------------------------------ đặc tính ----------------
        [HttpGet]
        public IActionResult Edit_ChemistryProperty(int id, string type)
        {

            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            ViewBag.Layout = Subject_name;



            List<EquipmentProperty> equipment = new List<EquipmentProperty>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "EquipmentProperty/Get/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<EquipmentProperty>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = type;
            ViewBag.idEquipment = id;
            return View(equipment);
        }

        public ActionResult Delete_ChemistryProperty(int id, string type, int idEquipment)
        {

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                try
                {
                    HttpResponseMessage response;
                    StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "EquipmentProperty/Delete/" + idEquipment, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = type;
                        return RedirectToAction("Edit_ChemistryProperty", new { id, type });
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = type;
                    return RedirectToAction("Edit_ChemistryProperty", new { id, type });
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("Edit_ChemistryProperty", new { id, type });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("Edit_ChemistryProperty", new { id, type });
            }

        }

        public ActionResult Create_ChemistryProperty(int id, string type, IFormFile File)
        {
            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                if (File != null)
                {
                    EquipmentProperty equipmentProperty = new EquipmentProperty();
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
                    equipmentProperty.Image = pathFolderSave + "FileFolder/Equipment/" + uniqueFileName;


                    try
                    {
                        equipmentProperty.EquipmentId = id;
                        HttpResponseMessage response;
                        string data = JsonConvert.SerializeObject(equipmentProperty);
                        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                        response = _httpClient.PostAsync(_httpClient.BaseAddress + "EquipmentProperty/Insert", content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            ViewBag.ActiveMenu = "subject6";
                            ViewBag.ActiveSubMenu = "dutru";
                            ViewBag.ActiveSubMenuLv2 = type;
                            return RedirectToAction("Edit_ChemistryProperty", new { id, type });
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["errorMessage"] = ex.Message;
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "dutru";
                        ViewBag.ActiveSubMenuLv2 = type;
                        return RedirectToAction("Edit_ChemistryProperty", new { id, type });
                    }
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("Edit_ChemistryProperty", new { id, type });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("Edit_ChemistryProperty", new { id, type });
            }

        }

        //----------------------------hóa chất ------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Chemistry(string type)
        {

            if (_hasError || userLogin.RoleName == "User")
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            ViewBag.Layout = Subject_name;



            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/" + Subject_id + "/" + type).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = type;
            return View(equipment);
        }


        [HttpGet]
        public ActionResult Edit_Chemistry(int id, string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


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
                ViewBag.ActiveSubMenuLv2 = type;
                return Ok();
            }

            if (userLogin.RoleName == "Technical" || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return View(equipment);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_Chemistry", new { type });
            }
        }
        [HttpPost]
        public ActionResult Edit_Chemistry(Equipment equipment, IFormFile File, string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


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
                    ViewBag.ActiveSubMenuLv2 = type;
                    return RedirectToAction("GetAll_Chemistry", new { type });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create_Chemistry(string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Technical")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_Chemistry", new { type });
            }
        }
        [HttpPost]
        public ActionResult Create_Chemistry(Equipment equipment, IFormFile File, IFormFile coverImage, string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


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
                equipment.Type = type;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.Unit1Amount = 0;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = type;
                    return RedirectToAction("GetAll_Chemistry", new { type });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = type;
            return View();
        }


        public ActionResult Delete_Chemistry(int id, string type)
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
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_Chemistry", new { type });
            }

            if (userLogin.RoleName == "Technical" || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
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
                        ViewBag.ActiveSubMenuLv2 = type;
                        return RedirectToAction("GetAll_Chemistry", new { type });
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "dutru";
                    ViewBag.ActiveSubMenuLv2 = type;
                    return RedirectToAction("GetAll_Chemistry", new { type });
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_Chemistry", new { type });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "dutru";
                ViewBag.ActiveSubMenuLv2 = type;
                return RedirectToAction("GetAll_Chemistry", new { type });
            }

        }


        [HttpGet]
        public ActionResult Details_Chemistry(int id, string type)
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


            Equipment equipment = new Equipment();
            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            List<EquipmentProperty> equipmentProperty = new List<EquipmentProperty>();
            HttpResponseMessage response_pro = _httpClient.GetAsync(_httpClient.BaseAddress + "EquipmentProperty/Get/" + id).Result;
            if (response_pro.IsSuccessStatusCode)
            {
                string data = response_pro.Content.ReadAsStringAsync().Result;
                equipmentProperty = JsonConvert.DeserializeObject<List<EquipmentProperty>>(data);
            }

            ViewBag.equipmentProperty = equipmentProperty;

            List<WareHouse> wareHouses = new List<WareHouse>();

            HttpResponseMessage response1 = _httpClient.GetAsync(_httpClient.BaseAddress + "WareHouseControler/GetWareHouseByEquipmentId/" + id).Result;

            if (response1.IsSuccessStatusCode)
            {
                string data = response1.Content.ReadAsStringAsync().Result;
                wareHouses = JsonConvert.DeserializeObject<List<WareHouse>>(data);
            }
            ViewBag.wareHouses = wareHouses;

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "dutru";
            ViewBag.ActiveSubMenuLv2 = type;
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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


        //----------------------------- Tin tức ------------------------------------------

        [HttpGet]
        public IActionResult GetAll_New()
        {
            if (_hasError)
            {
                return View("Error");
            }

            TempData["name"] = userLogin.Name;
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByType/" + userLogin.SchoolId + "/" + Subject_id + "/" + titleDocument).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenuLv2 = titleDocument;
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


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
                    ViewBag.ActiveSubMenuLv2 = titleDocument;
                    return RedirectToAction("GetAll_Document", new { titleDocument });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenuLv2 = titleDocument;
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


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
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return Ok();
            }

            if (userLogin.Email == document.CreateBy || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return View(document);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


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
                    ViewBag.ActiveSubMenuLv2 = titleDocument;
                    return RedirectToAction("GetAll_Document", new { titleDocument });
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return View();
            }
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenuLv2 = titleDocument;
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
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
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
                        ViewBag.ActiveSubMenuLv2 = titleDocument;
                        return RedirectToAction("GetAll_Document", new { titleDocument });
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenuLv2 = titleDocument;
                    return RedirectToAction("GetAll_Document", new { titleDocument });
                }
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenuLv2 = titleDocument;
                return RedirectToAction("GetAll_Document", new { titleDocument });
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;


            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenuLv2 = titleDocument;
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
            ViewBag.MenuItems = manageTitles;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath; TempData["subject_1"] = subject1.SubjectName; ViewBag.Layout = Subject_name;

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