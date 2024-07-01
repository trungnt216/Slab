using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Text;

namespace SaRLAB.AdminWeb.Controllers
{
    public class Subject_6 : Controller
    {
        string pathFolderSave = Program.FilePath;

        private readonly IWebHostEnvironment _env;


        Uri baseAddress = new Uri(Program.api);

        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        int Subject_id = 11;

        Subject subject1 = new Subject();
        Subject subject2 = new Subject();
        Subject subject3 = new Subject();
        Subject subject4 = new Subject();
        Subject subject5 = new Subject();
        Subject subject6 = new Subject();

        public Subject_6(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

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

            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/6").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject1 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub2 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/7").Result;
            if (response_sub2.IsSuccessStatusCode)
            {
                string data = response_sub2.Content.ReadAsStringAsync().Result;
                subject2 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub3 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/8").Result;
            if (response_sub3.IsSuccessStatusCode)
            {
                string data = response_sub3.Content.ReadAsStringAsync().Result;
                subject3 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub4 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/9").Result;
            if (response_sub4.IsSuccessStatusCode)
            {
                string data = response_sub4.Content.ReadAsStringAsync().Result;
                subject4 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub5 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/10").Result;
            if (response_sub5.IsSuccessStatusCode)
            {
                string data = response_sub5.Content.ReadAsStringAsync().Result;
                subject5 = JsonConvert.DeserializeObject<Subject>(data);
            }
            HttpResponseMessage response_sub6 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/11").Result;
            if (response_sub6.IsSuccessStatusCode)
            {
                string data = response_sub6.Content.ReadAsStringAsync().Result;
                subject6 = JsonConvert.DeserializeObject<Subject>(data);
            }

        }
        //------------------------------ sửa tên logo icon ----------------------------------------
        [HttpGet]
        public IActionResult Configuration_Subject()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            Subject subject = new Subject();
            HttpResponseMessage response_sub1 = _httpClient.GetAsync(_httpClient.BaseAddress + "Subject/GetByID/11").Result;
            if (response_sub1.IsSuccessStatusCode)
            {
                string data = response_sub1.Content.ReadAsStringAsync().Result;
                subject = JsonConvert.DeserializeObject<Subject>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "configurationsubject6";
            ViewBag.ActiveSubMenuLv2 = "configurationsubject6";
            return View(subject);
        }

        [HttpPost]
        public IActionResult Configuration_Subject(Subject subject_new)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            if (userLogin.RoleName == "Owner")
            {
                try
                {
                    string data = JsonConvert.SerializeObject(subject_new);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Subject/update", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["successMessage"] = "create success";

                        return RedirectToAction("Configuration_Subject");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveSubMenuLv2 = "internationalLevel";
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "configurationsubject6";
                    ViewBag.ActiveSubMenuLv2 = "configurationsubject6";
                    return View();
                }
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "configurationsubject6";
                ViewBag.ActiveSubMenuLv2 = "configurationsubject6";
                return View();
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "configurationsubject6";
            ViewBag.ActiveSubMenuLv2 = "configurationsubject6";
            return View();
        }

        //------------------------- thực nghiệm -------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Experiment()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/EXPERIMENT").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtuallabsubject6";
            ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
            return View(documents);
        }

        public ActionResult Accept_Experiment(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                return RedirectToAction("GetAll_Experiment");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.PageFlag = true;
                    document.UpdateTime = DateTime.Now;
                    document.UpdateBy = userLogin.Email;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtuallabsubject6";
                        ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtuallabsubject6";
                    ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                return RedirectToAction("GetAll_Experiment");
            }
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
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveSubMenu = "virtuallabsubject6";
                        ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtuallabsubject6";
                    ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
                return RedirectToAction("GetAll_Experiment");
            }
        }


        [HttpGet]
        public ActionResult Details_Experiment(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtuallabsubject6";
            ViewBag.ActiveSubMenuLv2 = "experimentsubject6";
            return View(document);
        }

        //-------------------------------------- đại cương (Conspectus) ---------------------------------------------

        [HttpGet]
        public IActionResult GetAll_Conspectus()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/CONSPECTUS").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtuallabsubject6";
            ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
            return View(documents);
        }

        public ActionResult Accept_Conspectus(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                return RedirectToAction("GetAll_Conspectus");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.PageFlag = true;
                    document.UpdateTime = DateTime.Now;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtuallabsubject6";
                        ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                        return RedirectToAction("GetAll_Conspectus");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtuallabsubject6";
                    ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                    return RedirectToAction("GetAll_Conspectus");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                return RedirectToAction("GetAll_Conspectus");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                return RedirectToAction("GetAll_Conspectus");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                return RedirectToAction("GetAll_Conspectus");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtuallabsubject6";
                        ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                        return RedirectToAction("GetAll_Conspectus");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtuallabsubject6";
                    ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                    return RedirectToAction("GetAll_Conspectus");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                return RedirectToAction("GetAll_Conspectus");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
                return RedirectToAction("GetAll_Conspectus");
            }
        }


        [HttpGet]
        public ActionResult Details_Conspectus(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtuallabsubject6";
            ViewBag.ActiveSubMenuLv2 = "conspectussubject6";
            return View(document);
        }

        //--------------------------------hoạt tính sinh học---- subject1logicalsubject1 ------------------------------------

        [HttpGet]
        public IActionResult GetAll_subject1logical()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/subject1LOGICAL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtuallabsubject6";
            ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
            return View(documents);
        }


        public ActionResult Accept_subject1logical(int id)
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
                return RedirectToAction("GetAll_subject1logical");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtuallabsubject6";
                        ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                        return RedirectToAction("GetAll_subject1logical");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtuallabsubject6";
                    ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                    return RedirectToAction("GetAll_subject1logical");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                return RedirectToAction("GetAll_subject1logical");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                return RedirectToAction("GetAll_subject1logical");
            }
        }

        public ActionResult Delete_subject1logical(int id)
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
                return RedirectToAction("GetAll_subject1logical");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "virtuallabsubject6";
                        ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                        return RedirectToAction("GetAll_subject1logical");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "virtuallabsubject6";
                    ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                    return RedirectToAction("GetAll_subject1logical");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                return RedirectToAction("GetAll_subject1logical");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "virtuallabsubject6";
                ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
                return RedirectToAction("GetAll_subject1logical");
            }
        }


        [HttpGet]
        public ActionResult Details_subject1logical(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "virtuallabsubject6";
            ViewBag.ActiveSubMenuLv2 = "subject1logicalsubject1";
            return View(document);
        }

        //-------------------------------Từ Vựng ----- Vocabulary ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Vocabulary()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/VOCABULARY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "tienganhsubject6";
            ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
            return View(documents);
        }


        public ActionResult Accept_Vocabulary(int id)
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
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "tienganhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                        return RedirectToAction("GetAll_Vocabulary");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "tienganhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                    return RedirectToAction("GetAll_Vocabulary");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                return RedirectToAction("GetAll_Vocabulary");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                return RedirectToAction("GetAll_Vocabulary");
            }
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "tienganhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                        return RedirectToAction("GetAll_Vocabulary");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "tienganhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                    return RedirectToAction("GetAll_Vocabulary");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                return RedirectToAction("GetAll_Vocabulary");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
                return RedirectToAction("GetAll_Vocabulary");
            }
        }


        [HttpGet]
        public ActionResult Details_Vocabulary(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "tienganhsubject6";
            ViewBag.ActiveSubMenuLv2 = "vocabularysubject6";
            return View(document);
        }

        //-------------------------------Bài tập ----- Exam ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Exam()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/EXAM").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "tienganhsubject6";
            ViewBag.ActiveSubMenuLv2 = "examsubject6";
            return View(documents);
        }


        public ActionResult Accept_Exam(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examsubject6";
                return RedirectToAction("GetAll_Exam");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "tienganhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "examsubject6";
                        return RedirectToAction("GetAll_Exam");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "tienganhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "examsubject6";
                    return RedirectToAction("GetAll_Exam");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examsubject6";
                return RedirectToAction("GetAll_Exam");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examsubject6";
                return RedirectToAction("GetAll_Exam");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examsubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "tienganhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "examsubject6";
                        return RedirectToAction("GetAll_Exam");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "tienganhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "examsubject6";
                    return RedirectToAction("GetAll_Exam");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examsubject6";
                return RedirectToAction("GetAll_Exam");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examsubject6";
                return RedirectToAction("GetAll_Exam");
            }
        }


        [HttpGet]
        public ActionResult Details_Exam(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "tienganhsubject6";
            ViewBag.ActiveSubMenuLv2 = "examsubject6";
            return View(document);
        }

        //-------------------------------Bài tập song ngữ ----- Examenglish ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Examenglish()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/EXAMENG").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "tienganhsubject6";
            ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
            return View(documents);
        }


        public ActionResult Accept_Examenglish(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                return RedirectToAction("GetAll_Examenglish");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "tienganhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                        return RedirectToAction("GetAll_Examenglish");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "tienganhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                    return RedirectToAction("GetAll_Examenglish");
                }
                return RedirectToAction("GetAll_Examenglish");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                return RedirectToAction("GetAll_Examenglish");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                return RedirectToAction("GetAll_Examenglish");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "tienganhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                        return RedirectToAction("GetAll_Examenglish");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "tienganhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                    return RedirectToAction("GetAll_Examenglish");
                }
                return RedirectToAction("GetAll_Examenglish");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "tienganhsubject6";
                ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
                return RedirectToAction("GetAll_Examenglish");
            }
        }


        [HttpGet]
        public ActionResult Details_Examenglish(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "tienganhsubject6";
            ViewBag.ActiveSubMenuLv2 = "examenglishsubject6";
            return View(document);
        }

        //-------------------------------Đề tài cấp sở----- Department_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Department_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/DEPARTMENTLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
            return View(documents);
        }


        public ActionResult Accept_Department_level(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                return RedirectToAction("GetAll_Department_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                        return RedirectToAction("GetAll_Department_level");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAll_Department_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                return RedirectToAction("GetAll_Department_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                return RedirectToAction("GetAll_Department_level");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                        return RedirectToAction("GetAll_Department_level");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAll_Department_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                return RedirectToAction("GetAll_Department_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
                return RedirectToAction("GetAll_Department_level");
            }
        }


        [HttpGet]
        public ActionResult Details_Department_level(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "departmentLevelsubject6";
            return View(document);
        }

        //-------------------------------Đề tài cấp tỉnh----- Provincial_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Provincial_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/PROVONCIALLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
            return View(documents);
        }


        public ActionResult Accept_Provincial_level(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                return RedirectToAction("GetAll_Provincial_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                        return RedirectToAction("GetAll_Provincial_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                    return RedirectToAction("GetAll_Provincial_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                return RedirectToAction("GetAll_Provincial_level");
            }
            else
            {
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                TempData["notice"] = "Bạn không có quyền duyệt!";
                return RedirectToAction("GetAll_Provincial_level");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                        return RedirectToAction("GetAll_Provincial_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                    return RedirectToAction("GetAll_Provincial_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
                return RedirectToAction("GetAll_Provincial_level");
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
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
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "provincialLevelsubject6";
            return View(document);
        }


        //-------------------------------Đề tài cấp quốc gia----- National_level topic ------------------------------------

        [HttpGet]
        public IActionResult GetAll_National_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
            return View(documents);
        }


        public ActionResult Accept_National_level(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
                return RedirectToAction("GetAll_National_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
                        return RedirectToAction("GetAll_National_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
                    return RedirectToAction("GetAll_National_level");
                }
                return RedirectToAction("GetAll_National_level");
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
                TempData["notice"] = "Bạn không có quyền duyệt!";
                return RedirectToAction("GetAll_National_level");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
                        return RedirectToAction("GetAll_National_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
                    return RedirectToAction("GetAll_National_level");
                }
                return RedirectToAction("GetAll_National_level");
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
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
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "nationalLevelsubject6";
            return View(document);
        }

        //------------------------------- câu hỏi chuẩn bị ----- Preparation_questions ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Preparation_questions()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/PREPARATIONQUESTION").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "thuchanhsubject6";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
            return View(documents);
        }


        public ActionResult Accept_Preparation_questions(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                return RedirectToAction("GetAll_Preparation_questions");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "thuchanhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                        return RedirectToAction("GetAll_Preparation_questions");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "thuchanhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                return RedirectToAction("GetAll_Preparation_questions");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                return RedirectToAction("GetAll_Preparation_questions");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "thuchanhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                        return RedirectToAction("GetAll_Preparation_questions");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "thuchanhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                return RedirectToAction("GetAll_Preparation_questions");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
                return RedirectToAction("GetAll_Preparation_questions");
            }
        }


        [HttpGet]
        public ActionResult Details_Preparation_questions(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "thuchanhsubject6";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestionssubject6";
            return View(document);
        }


        //------------------------------- báo cáo thực hành ----- Practice_report------------------------------------

        [HttpGet]
        public IActionResult GetAll_Practice_report()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "thuchanhsubject6";
            ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
            return View(documents);
        }


        public ActionResult Accept_Practice_report(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice_report");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "thuchanhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                        return RedirectToAction("GetAll_Practice_report");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "thuchanhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                    return RedirectToAction("GetAll_Practice_report");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                return RedirectToAction("GetAll_Practice_report");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                return RedirectToAction("GetAll_Practice_report");
            }
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "thuchanhsubject6";
                        ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                        return RedirectToAction("GetAll_Practice_report");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "thuchanhsubject6";
                    ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                    return RedirectToAction("GetAll_Practice_report");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                return RedirectToAction("GetAll_Practice_report");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "thuchanhsubject6";
                ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
                return RedirectToAction("GetAll_Practice_report");
            }
        }


        [HttpGet]
        public ActionResult Details_Practice_report(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "thuchanhsubject6";
            ViewBag.ActiveSubMenuLv2 = "practiceReportsubject6";
            return View(document);
        }

        //------------------------------- đề tài cấp quốc tế ----- International_level------------------------------------

        [HttpGet]
        public IActionResult GetAll_International_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/INTERNATIONAL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
            return View(documents);
        }


        public ActionResult Accept_International_level(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_International_level");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                        return RedirectToAction("GetAll_International_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                    return RedirectToAction("GetAll_International_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                return RedirectToAction("GetAll_International_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                return RedirectToAction("GetAll_International_level");
            }
        }

        public ActionResult Delete_International_level(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_International_level");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject6";
                        ViewBag.ActiveSubMenu = "nghiencuusubject6";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                        return RedirectToAction("GetAll_International_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "nghiencuusubject6";
                    ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                    return RedirectToAction("GetAll_International_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                return RedirectToAction("GetAll_International_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "nghiencuusubject6";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
                return RedirectToAction("GetAll_International_level");
            }
        }


        [HttpGet]
        public ActionResult Details_International_level(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "nghiencuusubject6";
            ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject6";
            return View(document);
        }

        //--------------------- mutiple choice - ---------------------------------
        [HttpGet]
        public IActionResult GetAll_Question()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/" + Subject_id).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
            ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
            return View(equipment);
        }

        [HttpGet]
        public ActionResult Create_Question()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                return RedirectToAction("GetAll_Question");
            }
        }
        [HttpPost]
        public ActionResult Create_Question(Quiz quiz, IFormFile QuestionFile, IFormFile OptionAFile, IFormFile OptionBFile, IFormFile OptionCFile, IFormFile OptionDFile)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            if (QuestionFile != null && quiz.QuestionImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(QuestionFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    QuestionFile.CopyTo(stream);
                }
                quiz.QuestionImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionAFile != null && quiz.OptionAImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionAFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionAFile.CopyTo(stream);
                }
                quiz.OptionAImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionBFile != null && quiz.OptionBImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionBFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionBFile.CopyTo(stream);
                }
                quiz.OptionBImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionCFile != null && quiz.OptionCImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionCFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionCFile.CopyTo(stream);
                }
                quiz.OptionCImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionDFile != null && quiz.OptionDImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionDFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionDFile.CopyTo(stream);
                }
                quiz.OptionDImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            try
            {
                quiz.SchoolId = userLogin.SchoolId;
                quiz.SubjectId = Subject_id;

                string data = JsonConvert.SerializeObject(quiz);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["notice"] = "Thêm câu hỏi thành công";
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject6";
                    ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                    ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
            ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Question(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Quiz quiz = new Quiz();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetQuizById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                quiz = JsonConvert.DeserializeObject<Quiz>(data);
            }

            if (quiz == null)
            {
                TempData["notice"] = "Không tìm thấy dữ liệu";
            }

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                return View(quiz);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                return RedirectToAction("GetAll_Question");
            }
        }
        [HttpPost]
        public ActionResult Edit_Question(Quiz quiz, IFormFile QuestionFile, IFormFile OptionAFile, IFormFile OptionBFile, IFormFile OptionCFile, IFormFile OptionDFile)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            if (QuestionFile != null && quiz.QuestionImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(QuestionFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    QuestionFile.CopyTo(stream);
                }
                quiz.QuestionImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionAFile != null && quiz.OptionAImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionAFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionAFile.CopyTo(stream);
                }
                quiz.OptionAImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionBFile != null && quiz.OptionBImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionBFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionBFile.CopyTo(stream);
                }
                quiz.OptionBImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionCFile != null && quiz.OptionCImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionCFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionCFile.CopyTo(stream);
                }
                quiz.OptionCImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            if (OptionDFile != null && quiz.OptionDImage == null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "FileFolder/Quizz");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(OptionDFile.FileName);

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    OptionDFile.CopyTo(stream);
                }
                quiz.OptionDImage = pathFolderSave + "FileFolder/Quizz/" + uniqueFileName;
            }

            try
            {
                quiz.SchoolId = userLogin.SchoolId;
                quiz.SubjectId = 3;

                string data = JsonConvert.SerializeObject(quiz);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Update/" + quiz.ID, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject6";
                ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
                ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject6";
            ViewBag.ActiveSubMenu = "cauhoiantoansubject6";
            ViewBag.ActiveSubMenuLv2 = "cauhoisubject6";
            return View();
        }

        public ActionResult Delete_Question(int id)
        {
            try
            {
                HttpResponseMessage response;
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Delete/" + id, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("GetAll_Question");
            }
            return RedirectToAction("GetAll_Question");
        }
        //------------------------------- lý thuyết - theory ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Theory()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/THEORY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "theory1";
            return View(documents);
        }


        public ActionResult Accept_Theory(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Theory");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject1";
                        return RedirectToAction("GetAll_Theory");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_Theory");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Theory");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Theory");
            }
        }

        public ActionResult Delete_Theory(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Theory");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "theory1";
                        return RedirectToAction("GetAll_Theory");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_Theory");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Theory");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Theory");
            }
        }


        [HttpGet]
        public ActionResult Details_Theory(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "theory1";
            return View(document);
        }

        //------------------------------- thực hành - pratice ------------------------------------

        [HttpGet]
        public IActionResult GetAll_Practice()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/PRACTICE").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "practice1";
            return View(documents);
        }


        public ActionResult Accept_Pratice(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "pratice1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject1";
                        return RedirectToAction("GetAll_Practice");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_Practice");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Theory");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Practice");
            }
        }

        public ActionResult Delete_Practice(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "theory1";
                        return RedirectToAction("GetAll_Practice");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_Practice");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Practice");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Practice");
            }
        }


        [HttpGet]
        public ActionResult Details_Practice(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "theory1";
            return View(document);
        }

        //------------------------------- lý thuyết - theory ------------------------------------

        [HttpGet]
        public IActionResult GetAll_ViTheory()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/VITHEORY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "theory1";
            return View(documents);
        }


        public ActionResult Accept_ViTheory(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Theory");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject1";
                        return RedirectToAction("GetAll_ViTheory");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_ViTheory");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_ViTheory");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_ViTheory");
            }
        }

        public ActionResult Delete_ViTheory(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_ViTheory");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "theory1";
                        return RedirectToAction("GetAll_ViTheory");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_ViTheory");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_ViTheory");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_ViTheory");
            }
        }


        [HttpGet]
        public ActionResult Details_ViTheory(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "theory1";
            return View(document);
        }

        //------------------------------- thực hành - pratice ------------------------------------

        [HttpGet]
        public IActionResult GetAll_ViPractice()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/" + Subject_id + "/VIPRACTICE").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "practice1";
            return View(documents);
        }


        public ActionResult Accept_ViPratice(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "pratice1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice");
            }

            if (document.CreateBy == userLogin.Email || userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                try
                {
                    document.UpdateTime = DateTime.Now;
                    document.PageFlag = true;

                    string data = JsonConvert.SerializeObject(document);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Update/" + document.ID, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelsubject1";
                        return RedirectToAction("GetAll_Practice");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_Practice");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Theory");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Practice");
            }
        }

        public ActionResult Delete_ViPractice(int id)
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
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "subject1";
                        ViewBag.ActiveSubMenu = "virtual1";
                        ViewBag.ActiveSubMenuLv2 = "theory1";
                        return RedirectToAction("GetAll_Practice");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "subject1";
                    ViewBag.ActiveSubMenu = "virtual1";
                    ViewBag.ActiveSubMenuLv2 = "theory1";
                    return RedirectToAction("GetAll_Practice");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Practice");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "subject1";
                ViewBag.ActiveSubMenu = "virtual1";
                ViewBag.ActiveSubMenuLv2 = "theory1";
                return RedirectToAction("GetAll_Practice");
            }
        }


        [HttpGet]
        public ActionResult Details_ViPractice(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            TempData["subject_1"] = subject1.SubjectName;
            TempData["subject_2"] = subject2.SubjectName;
            TempData["subject_3"] = subject3.SubjectName;
            TempData["subject_4"] = subject4.SubjectName;
            TempData["subject_5"] = subject5.SubjectName;
            TempData["subject_6"] = subject6.SubjectName;
            Document document = new Document();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetById/" + id).Result;


            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                document = JsonConvert.DeserializeObject<Document>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "subject1";
            ViewBag.ActiveSubMenu = "virtual1";
            ViewBag.ActiveSubMenuLv2 = "theory1";
            return View(document);
        }

    }
}
