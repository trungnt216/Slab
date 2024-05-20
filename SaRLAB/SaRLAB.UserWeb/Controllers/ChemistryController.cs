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
            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/CHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            return View(equipment);
        }


        [HttpGet]
        public ActionResult Edit_Chemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            return View(equipment);
        }
        [HttpPost]
        public ActionResult Edit_Chemistry(Equipment equipment, IFormFile File) 
        {
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
                equipment.ImagePath = filePath;
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
                    return RedirectToAction("GetAll_Chemistry");
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
        public ActionResult Create_Chemistry()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create_Chemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.ImagePath = filePath;
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = 1;
                equipment.Type = "CHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }


        public IActionResult Delete_Chemistry(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_Chemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("GetAll_Chemistry");
            }
            return RedirectToAction("GetAll_Chemistry");
        }


        [HttpGet]
        public ActionResult Details_Chemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + userLogin.SchoolId + "/1/CHEMISTRY/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            return View(equipment);
        }

        //----------------------------dụng cụ-----------------------------------------------
        [HttpGet]
        public IActionResult GetAll_ToolChemistry()
        {
            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/TOOLCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_ToolChemistry()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create_ToolChemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.ImagePath = filePath;
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = 1;
                equipment.type = "TOOLCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAll_ToolChemistry");
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
        public ActionResult Edit_ToolChemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            return View(equipment);
        }
        [HttpPost]
        public ActionResult Edit_ToolChemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.ImagePath = filePath;
            }

            try
            {
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Update/" +  equipment.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Delete_ToolChemistry(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("GetAll_ToolChemistry");
            }
            return RedirectToAction("GetAll_ToolChemistry");
        }


        [HttpGet]
        public ActionResult Details_ToolChemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            return View(equipment);
        }


        //------------------------Thiết bị ----------------------------------------------
        public IActionResult GetAll_EquipmentChemistry()
        {
            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetAll/" + userLogin.SchoolId + "/1/EQUIPMENTCHEMISTRY").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            return View(equipment);
        }


        [HttpGet]
        public ActionResult Create_EquipmentChemistry()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create_EquipmentChemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.ImagePath = filePath;
            }

            try
            {
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateTime = DateTime.Now;
                equipment.UpdateBy = userLogin.Email;
                equipment.SchoolId = userLogin.SchoolId;
                equipment.SubjectId = 1;
                equipment.type = "EQUIPMENTCHEMISTRY";
                equipment.SchoolId = userLogin.SchoolId;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAll_ToolChemistry");
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
        public ActionResult Edit_EquipmentChemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            return View(equipment);
        }
        [HttpPost]
        public ActionResult Edit_EquipmentChemistry(Equipment equipment, IFormFile File)
        {
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
                equipment.ImagePath = filePath;
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
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Delete_EquipmentChemistry(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_ToolChemistry");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("GetAll_ToolChemistry");
            }
            return RedirectToAction("GetAll_ToolChemistry");
        }


        [HttpGet]
        public ActionResult Details_EquipmentChemistry(int id)
        {
            Equipment equipment = new Equipment();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<Equipment>(data);
            }

            return View(equipment);
        }

        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------
    }
}
