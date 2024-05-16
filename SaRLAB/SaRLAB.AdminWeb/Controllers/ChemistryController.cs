using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SaRLAB.AdminWeb.Controllers
{
    public class ChemistryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        public ChemistryController(ILogger<HomeController> logger, IConfiguration configuration)
        {
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
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }




        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult Edit_TopicScientificResearch(int id)
        {
            ScientificResearch sc = new ScientificResearch();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "ScientificResearch/GetById/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                sc = JsonConvert.DeserializeObject<ScientificResearch>(data);
            }

            return View(sc);
        }
        [HttpPost]
        public IActionResult Edit_TopicScientificResearch(ScientificResearch sc)
        {
            try
            {
                string data = JsonConvert.SerializeObject(sc);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "ScientificResearch/Update/" + sc.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Delete_TopicScientificResearch(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "ScientificResearch/Delete/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAll_ScientificResearch");
        }

        [HttpGet]
        public IActionResult Create_TopicScientificResearch()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create_TopicScientificResearch(ScientificResearch scientificResearch) 
        {
            if(scientificResearch == null)
            {
                return View();
            }

            try
            {
                scientificResearch.CreateBy = userLogin.Email;
                scientificResearch.CreateTime = DateTime.Now;
                scientificResearch.UpdateTime = DateTime.Now;
                scientificResearch.UpdateBy = userLogin.Email;
                scientificResearch.SubjectId = 1;

                string data = JsonConvert.SerializeObject(scientificResearch);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "ScientificResearch/Insert", content).Result;

                if(response.IsSuccessStatusCode) 
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAll_ScientificResearch");
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
        public IActionResult Detail_TopicScientificResearch(int id)
        {
            List<ScientificResearchFile> sc = new List<ScientificResearchFile>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "ScientificResearchFile/GetAll/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                sc = JsonConvert.DeserializeObject<List<ScientificResearchFile>>(data);
            }

            TempData["id"] = id;
            
            return View(sc);
        }

        //----------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_ScientificResearch()
        {
            List<ScientificResearch> scientificResearches = new List<ScientificResearch>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "ScientificResearch/1/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                scientificResearches = JsonConvert.DeserializeObject<List<ScientificResearch>>(data);
            }

            return View(scientificResearches);
        }


        [HttpGet]
        public IActionResult Create_ScientificResearch(int scId)
        {
            ScientificResearchFile sc = new ScientificResearchFile();
            sc.ScientificResearchId = scId;
            return View(sc);
        }
        [HttpPost]
        public IActionResult Create_ScientificResearch(ScientificResearchFile sc, IFormFile File)
        {
            if (File != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "FileFolder/ScientificResearchFile");

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
                sc.Path = filePath;
                sc.Type = File.GetType().ToString();
            }

            try
            {
                sc.CreateBy = userLogin.Email;
                sc.CreateTime = DateTime.Now;
                sc.UpdateTime = DateTime.Now;
                sc.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(sc);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "ScientificResearchFile/Insert", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("Detail_TopicScientificResearch", new { id = sc.ScientificResearchId });
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
        public IActionResult Edit_FileScientificResearch(int id)
        {
            ScientificResearchFile sc = new ScientificResearchFile();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "ScientificResearchFile/GetById/" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                sc = JsonConvert.DeserializeObject<ScientificResearchFile>(data);
            }

            return View(sc);
        }
        [HttpPost]
        public IActionResult Edit_FileScientificResearch(ScientificResearchFile sc, IFormFile File)
        {
            if(File != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "FileFolder/ScientificResearchFile");

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
                sc.Path = filePath;
                sc.Type = File.GetType().ToString();
            }

            try
            {
                sc.UpdateTime = DateTime.Now;
                sc.UpdateBy = userLogin.Email;

                string data = JsonConvert.SerializeObject(sc);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "ScientificResearchFile/Update/" +sc.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "create success";
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch(Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Delete_FileScientificResearch(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "ScientificResearchFile/Delete/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAll_ScientificResearch");
        }

        
        //--------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Equipment()
        {
            List<Equipment> equipment = new List<Equipment>();

            HttpResponseMessage response;
            response = _httpClient.GetAsync(_httpClient.BaseAddress + "Equipment/GetBySubject/1").Result;

            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Equipment>>(data);
            }

            return View(equipment);
        }

        [HttpGet]
        public IActionResult Create_Equipment()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create_Equipment(Equipment equipment)
        {
            try
            {
                equipment.UpdateTime = DateTime.Now;
                equipment.CreateTime = DateTime.Now;
                equipment.CreateBy = userLogin.Email;
                equipment.UpdateBy = userLogin.Email;
                equipment.SubjectId = 1;

                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Insert", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Status"] = "insert success";
                    return RedirectToAction("GetAll_Equipment");
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = $"{ex.Message}";
                return View();
            }

            return View();
        }

        [HttpGet]
        public IActionResult Edit_Equipment(int id)
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
        public IActionResult Edit_Equipment(Equipment equipment) 
        {
            equipment.UpdateTime = DateTime.Now;
            equipment.UpdateBy = userLogin.Email;
            equipment.SubjectId = 1;
            try
            {
                string data = JsonConvert.SerializeObject(equipment);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Equipment/Update/" + equipment.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "Banner update success";
                    return RedirectToAction("GetAll_Equipment");
                }

            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }

        public IActionResult Delete_Equipment(int id)
        {
            try
            {
                Console.WriteLine(id.ToString());
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "Equipment/Delete/" + id).Result;

                Console.WriteLine(response);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_Equipment");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAll_Equipment");
        }
        
        
        //------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_PlanDetail() 
        {
            List<PlanDetail> planDetail = new List<PlanDetail>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "").Result;

            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                planDetail = JsonConvert.DeserializeObject<List<PlanDetail>>(data);
            }

            return View(planDetail);        
        }

        [HttpGet]
        public IActionResult Create_PlanDetail()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create_PlanDetail(PlanDetail planDetail)
        {
            if(planDetail == null)
            {
                return View();
            }
            try
            {
                planDetail.CreateTime = DateTime.Now;
                planDetail.UpdateTime = DateTime.Now;
                planDetail.CreateBy = userLogin.Email;
                planDetail.UpdateBy = userLogin.Email;

                string data =JsonConvert.SerializeObject(planDetail);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAll_PlanDetail");
                }
            }
            catch(Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

            return View();
        }


        [HttpGet]        
        public IActionResult Edit_PlanDetail(int  id)
        {

            PlanDetail planDetail = new PlanDetail();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "" + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                planDetail = JsonConvert.DeserializeObject<PlanDetail>(data);
            }

            return View(planDetail);
        }
        [HttpPost]
        public IActionResult Edit_PlanDetail(PlanDetail planDetail)
        {
            try
            {
                string data = JsonConvert.SerializeObject(planDetail);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "" + planDetail.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User create success";
                    return RedirectToAction("GetAll_ScientificResearch");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return View();
        }


        public IActionResult Delete_PlanDetail(int id)
        {
            try
            {
                HttpResponseMessage response;
                response = _httpClient.DeleteAsync(_httpClient.BaseAddress + "" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_PlanDetail");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }
            return RedirectToAction("GetAll_PlanDetail");
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_PracticePlan()
        {
            List<PracticePlan> practicePlans = new List<PracticePlan>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                practicePlans = JsonConvert.DeserializeObject<List<PracticePlan>>(data);
            }

            return View(practicePlans);
        }

        public IActionResult Safety_PracticePlan()
        {
            return View();
        }

        public IActionResult Point_PracticePlan()
        {
            return View();
        }
    }
}
