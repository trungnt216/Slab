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
    public class PhysicsController : Controller
    {
        string pathFolderSave = Program.FilePath;

        private readonly IWebHostEnvironment _env;


        Uri baseAddress = new Uri(Program.api);
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        UserDto userLogin = new UserDto();

        public PhysicsController(ILogger<HomeController> logger, IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _configuration = configuration;

            string jwtToken = Program.jwtToken;

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.ReadJwtToken(jwtToken);

            foreach(Claim claim in token.Claims)
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

        //------------------------- thực nghiệm -------------------------------------------------------------------
        [HttpGet]
        public IActionResult GetAll_Experiment()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/EXPERIMENT").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "virtuallabphys";
            ViewBag.ActiveSubMenuLv2 = "experimentphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "experimentphys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "virtuallabphys";
                        ViewBag.ActiveSubMenuLv2 = "experimentphys";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "virtuallabphys";
                    ViewBag.ActiveSubMenuLv2 = "experimentphys";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "experimentphys";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "experimentphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "experimentphys";
                return RedirectToAction("GetAll_Experiment");
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveSubMenu = "virtuallabphys";
                        ViewBag.ActiveSubMenuLv2 = "experimentphys";
                        return RedirectToAction("GetAll_Experiment");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "virtuallabphys";
                    ViewBag.ActiveSubMenuLv2 = "experimentphys";
                    return RedirectToAction("GetAll_Experiment");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "experimentphys";
                return RedirectToAction("GetAll_Experiment");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "experimentphys";
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

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "virtuallabphys";
            ViewBag.ActiveSubMenuLv2 = "experimentphys";
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

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/CONSPECTUS").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "virtuallabphys";
            ViewBag.ActiveSubMenuLv2 = "conspectusphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "conspectusphys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "virtuallabphys";
                        ViewBag.ActiveSubMenuLv2 = "conspectusphys";
                        return RedirectToAction("GetAll_Conspectus");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "virtuallabphys";
                    ViewBag.ActiveSubMenuLv2 = "conspectusphys";
                    return RedirectToAction("GetAll_Conspectus");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "conspectusphys";
                return RedirectToAction("GetAll_Conspectus");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "conspectusphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "conspectusphys";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "virtuallabphys";
                        ViewBag.ActiveSubMenuLv2 = "conspectusphys";
                        return RedirectToAction("GetAll_Conspectus");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "virtuallabphys";
                    ViewBag.ActiveSubMenuLv2 = "conspectusphys";
                    return RedirectToAction("GetAll_Conspectus");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "conspectusphys";
                return RedirectToAction("GetAll_Conspectus");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "conspectusphys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "virtuallabphys";
            ViewBag.ActiveSubMenuLv2 = "conspectusphys";
            return View(document);
        }

        //--------------------------------hoạt tính sinh học---- biologicalphys ------------------------------------

        [HttpGet]
        public IActionResult GetAll_biologicalphys()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/biologicalphys").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "virtuallabphys";
            ViewBag.ActiveSubMenuLv2 = "biologicalphys";
            return View(documents);
        }


       public ActionResult Accept_biologicalphys(int id)
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
                return RedirectToAction("GetAll_biologicalphys");
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "virtuallabphys";
                        ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                        return RedirectToAction("GetAll_biologicalphys");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "virtuallabphys";
                    ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                    return RedirectToAction("GetAll_biologicalphys");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                return RedirectToAction("GetAll_biologicalphys");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                return RedirectToAction("GetAll_biologicalphys");
            }
        }

        public ActionResult Delete_biologicalphys(int id)
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
                return RedirectToAction("GetAll_biologicalphys");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "virtuallabphys";
                        ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                        return RedirectToAction("GetAll_biologicalphys");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "virtuallabphys";
                    ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                    return RedirectToAction("GetAll_biologicalphys");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                return RedirectToAction("GetAll_biologicalphys");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "virtuallabphys";
                ViewBag.ActiveSubMenuLv2 = "biologicalphys";
                return RedirectToAction("GetAll_biologicalphys");
            }
        }


        [HttpGet]
        public ActionResult Details_biologicalphys(int id)
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "virtuallabphys";
            ViewBag.ActiveSubMenuLv2 = "biologicalphys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/VOCABULARY").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "tienganhphys";
            ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "tienganhphys";
                        ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
                        return RedirectToAction("GetAll_Vocabulary");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "tienganhphys";
                    ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
                    return RedirectToAction("GetAll_Vocabulary");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
                return RedirectToAction("GetAll_Vocabulary");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
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
                    response = _httpClient.PostAsync(_httpClient.BaseAddress + "Document/Delete/" + id,content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "tienganhphys";
                        ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
                        return RedirectToAction("GetAll_Vocabulary");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "tienganhphys";
                    ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
                    return RedirectToAction("GetAll_Vocabulary");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
                return RedirectToAction("GetAll_Vocabulary");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "tienganhphys";
            ViewBag.ActiveSubMenuLv2 = "vocabularyphys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/EXAM").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "tienganhphys";
            ViewBag.ActiveSubMenuLv2 = "examphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examphys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "tienganhphys";
                        ViewBag.ActiveSubMenuLv2 = "examphys";
                        return RedirectToAction("GetAll_Exam");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "tienganhphys";
                    ViewBag.ActiveSubMenuLv2 = "examphys";
                    return RedirectToAction("GetAll_Exam");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examphys";
                return RedirectToAction("GetAll_Exam");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examphys";
                return RedirectToAction("GetAll_Exam");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "tienganhphys";
                        ViewBag.ActiveSubMenuLv2 = "examphys";
                        return RedirectToAction("GetAll_Exam");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "tienganhphys";
                    ViewBag.ActiveSubMenuLv2 = "examphys";
                    return RedirectToAction("GetAll_Exam");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examphys";
                return RedirectToAction("GetAll_Exam");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examphys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "tienganhphys";
            ViewBag.ActiveSubMenuLv2 = "examphys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/EXAMENG").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "tienganhphys";
            ViewBag.ActiveSubMenuLv2 = "examenglishphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examenglishphys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "tienganhphys";
                        ViewBag.ActiveSubMenuLv2 = "examenglishphys";
                        return RedirectToAction("GetAll_Examenglish");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "tienganhphys";
                    ViewBag.ActiveSubMenuLv2 = "examenglishphys";
                    return RedirectToAction("GetAll_Examenglish");
                }
                return RedirectToAction("GetAll_Examenglish");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examenglishphys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examenglishphys";
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "tienganhphys";
                        ViewBag.ActiveSubMenuLv2 = "examenglishphys";
                        return RedirectToAction("GetAll_Examenglish");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "tienganhphys";
                    ViewBag.ActiveSubMenuLv2 = "examenglishphys";
                    return RedirectToAction("GetAll_Examenglish");
                }
                return RedirectToAction("GetAll_Examenglish");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "tienganhphys";
                ViewBag.ActiveSubMenuLv2 = "examenglishphys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "tienganhphys";
            ViewBag.ActiveSubMenuLv2 = "examenglishphys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/DEPARTMENTLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                        return RedirectToAction("GetAll_Department_level");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAll_Department_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                return RedirectToAction("GetAll_Department_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                return RedirectToAction("GetAll_Department_level");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                        return RedirectToAction("GetAll_Department_level");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                    TempData["errorMessage"] = ex.Message;
                    return RedirectToAction("GetAll_Department_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
                return RedirectToAction("GetAll_Department_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "departmentLevelPhys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/PROVONCIALLEVEL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                        return RedirectToAction("GetAll_Provincial_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                    return RedirectToAction("GetAll_Provincial_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                return RedirectToAction("GetAll_Provincial_level");
            }
            else
            {
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                return RedirectToAction("GetAll_Provincial_level");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                        return RedirectToAction("GetAll_Provincial_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                    return RedirectToAction("GetAll_Provincial_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
                return RedirectToAction("GetAll_Provincial_level");
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "provincialLevelPhys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
                        return RedirectToAction("GetAll_National_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
                    return RedirectToAction("GetAll_National_level");
                }
                return RedirectToAction("GetAll_National_level");
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
                return RedirectToAction("GetAll_National_level");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
                        return RedirectToAction("GetAll_National_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
                    return RedirectToAction("GetAll_National_level");
                }
                return RedirectToAction("GetAll_National_level");
            }
            else
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "nationalLevelPhys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/PREPARATIONQUESTION").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "thuchanhphys";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "thuchanhphys";
                        ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                        return RedirectToAction("GetAll_Preparation_questions");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "thuchanhphys";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                return RedirectToAction("GetAll_Preparation_questions");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                return RedirectToAction("GetAll_Preparation_questions");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "thuchanhphys";
                        ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                        return RedirectToAction("GetAll_Preparation_questions");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "thuchanhphys";
                    ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                    return RedirectToAction("GetAll_Preparation_questions");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
                return RedirectToAction("GetAll_Preparation_questions");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "thuchanhphys";
            ViewBag.ActiveSubMenuLv2 = "preparationQuestionsPhys";
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

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/NATIONALLEVER").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "thuchanhphys";
            ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "thuchanhphys";
                        ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                        return RedirectToAction("GetAll_Practice_report");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "thuchanhphys";
                    ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                    return RedirectToAction("GetAll_Practice_report");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                return RedirectToAction("GetAll_Practice_report");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                TempData["notice"] = "khong tim thay du lieu";
                return RedirectToAction("GetAll_Practice_report");
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
                        ViewBag.ActiveMenuMain = "subject";
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "thuchanhphys";
                        ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                        return RedirectToAction("GetAll_Practice_report");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "thuchanhphys";
                    ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                    return RedirectToAction("GetAll_Practice_report");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
                return RedirectToAction("GetAll_Practice_report");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "thuchanhphys";
                ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "thuchanhphys";
            ViewBag.ActiveSubMenuLv2 = "practiceReportPhys";
            return View(document);
        }

        //------------------------------- đề tài cấp quốc tế ----- International_level------------------------------------

        [HttpGet]
        public IActionResult GetAll_International_level()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Document> documents = new List<Document>();

            HttpResponseMessage responses = _httpClient.GetAsync(_httpClient.BaseAddress + "Document/GetAllByTypeToAccept/" + userLogin.SchoolId + "/5/INTERNATIONAL").Result;

            if (responses.IsSuccessStatusCode)
            {
                string data = responses.Content.ReadAsStringAsync().Result;
                documents = JsonConvert.DeserializeObject<List<Document>>(data);
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                        return RedirectToAction("GetAll_International_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                    return RedirectToAction("GetAll_International_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                return RedirectToAction("GetAll_International_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền duyệt!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
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
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
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
                        ViewBag.ActiveMenu = "physics";
                        ViewBag.ActiveSubMenu = "nghiencuuphys";
                        ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                        return RedirectToAction("GetAll_International_level");
                    }
                }
                catch (Exception ex)
                {
                    TempData["errorMessage"] = ex.Message;
                    ViewBag.ActiveMenuMain = "subject";
                    ViewBag.ActiveMenu = "physics";
                    ViewBag.ActiveSubMenu = "nghiencuuphys";
                    ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                    return RedirectToAction("GetAll_International_level");
                }
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                return RedirectToAction("GetAll_International_level");
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền xóa!";
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "physics";
                ViewBag.ActiveSubMenu = "nghiencuuphys";
                ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
                return RedirectToAction("GetAll_International_level");
            }
        }


        [HttpGet]
        public ActionResult Details_International_level(int id)
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
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "physics";
            ViewBag.ActiveSubMenu = "nghiencuuphys";
            ViewBag.ActiveSubMenuLv2 = "internationalLevelPhys";
            return View(document);
        }


        //--------------------- mutiple choice - ---------------------------------
        [HttpGet]
        public IActionResult GetAll_Question()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
            List<Quiz> equipment = new List<Quiz>();

            HttpResponseMessage response = _httpClient.GetAsync(_httpClient.BaseAddress + "Quiz/GetRandomQuizzes/" + userLogin.SchoolId + "/5").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                equipment = JsonConvert.DeserializeObject<List<Quiz>>(data);
            }

            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "phys";
            ViewBag.ActiveSubMenu = "cauhoiantoanphys";
            ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
            return View(equipment);
        }

        [HttpGet]
        public ActionResult Create_Question()
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner" || userLogin.RoleName == "Teacher")
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "phys";
                ViewBag.ActiveSubMenu = "cauhoiantoanphys";
                ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
                return View();
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền thêm mới";
                return RedirectToAction("GetAll_Question");
            }
        }
        [HttpPost]
        public ActionResult Create_Question(Quiz quiz, IFormFile QuestionFile, IFormFile OptionAFile, IFormFile OptionBFile, IFormFile OptionCFile, IFormFile OptionDFile)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                quiz.SubjectId = 5;

                string data = JsonConvert.SerializeObject(quiz);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Insert/", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["notice"] = "Thêm câu hỏi thành công";
                    return RedirectToAction("GetAll_Question");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "phys";
                ViewBag.ActiveSubMenu = "cauhoiantoanphys";
                ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "phys";
            ViewBag.ActiveSubMenu = "cauhoiantoanphys";
            ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
            return View();
        }

        [HttpGet]
        public ActionResult Edit_Question(int id)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                return RedirectToAction("GetAll_Question");
            }

            if (userLogin.RoleName == "Admin" || userLogin.RoleName == "Owner")
            {
                ViewBag.ActiveMenuMain = "subject";
                ViewBag.ActiveMenu = "phys";
                ViewBag.ActiveSubMenu = "cauhoiantoanphys";
                ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
                return View(quiz);
            }
            else
            {
                TempData["notice"] = "Bạn không có quyền chỉnh sửa!";
                return RedirectToAction("GetAll_Question");
            }
        }
        [HttpPost]
        public ActionResult Edit_Question(Quiz quiz, IFormFile QuestionFile, IFormFile OptionAFile, IFormFile OptionBFile, IFormFile OptionCFile, IFormFile OptionDFile)
        {
            TempData["name"] = userLogin.Name;
            TempData["role"] = userLogin.RoleName;
            TempData["AvtPath"] = userLogin.AvtPath;
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
                quiz.SubjectId = 5;

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
                ViewBag.ActiveMenu = "phys";
                ViewBag.ActiveSubMenu = "cauhoiantoanphys";
                ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
                return View();
            }
            ViewBag.ActiveMenuMain = "subject";
            ViewBag.ActiveMenu = "phys";
            ViewBag.ActiveSubMenu = "cauhoiantoanphys";
            ViewBag.ActiveSubMenuLv2 = "cauhoivatly";
            return View();
        }

        public ActionResult Delete_Question(int id)
        {
            try
            {
                HttpResponseMessage response;
                StringContent content = new StringContent("", Encoding.UTF8, "application/json");
                response = _httpClient.PostAsync(_httpClient.BaseAddress + "Quiz/Delete/" + id,content).Result;

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
    }
}
