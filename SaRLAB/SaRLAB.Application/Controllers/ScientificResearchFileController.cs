using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.ScientificResearchFileService;
using SaRLAB.DataAccess.Service.ScientificResearchService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScientificResearchFileController : Controller
    {
        private readonly IScientificResearchFileService _scientificResearchFileService;
        public ScientificResearchFileController(IScientificResearchFileService scientificResearchFileService)
        {
            _scientificResearchFileService = scientificResearchFileService;
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_scientificResearchFileService.DeleteScientificResearchFileById(id));
            }
        }

        [HttpGet]
        [Route("GetAll/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAllBySubject(int id)
        {
            return Ok(_scientificResearchFileService.GetFilesByScientificResearchId(id));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetById(int id)
        {
            return Ok(_scientificResearchFileService.GetScientificResearchFileId(id));
        }

        [HttpPost]
        [Route("Insert")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Insert(ScientificResearchFile sc)
        {
            if(sc == null)
            {
                return BadRequest("not have data");
            }
            else
            {
                return Ok(_scientificResearchFileService.InsertScientificResearchFile(sc));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Update(int id, ScientificResearchFile sc)
        {
            if(id == 0) 
            {
                return BadRequest("have not instance");

            }
            else
            {
                return Ok(_scientificResearchFileService.UpdateScientificResearchFileById(id,sc));
            }
        }

        [HttpGet]
        [Route("GetScientificResearchFileByType/{type}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetScientificResearchFileByType(string type)
        {
            return Ok(_scientificResearchFileService.GetScientificResearchFileByType(type));
        }
    }
}
