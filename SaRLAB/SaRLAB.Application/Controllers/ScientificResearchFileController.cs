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
        public IActionResult GetAllBySubject(int id)
        {
            return Ok(_scientificResearchFileService.GetFilesByScientificResearchId(id));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_scientificResearchFileService.GetScientificResearchFileId(id));
        }

        [HttpPost]
        [Route("Insert")]
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
        public IActionResult GetScientificResearchFileByType(string type)
        {
            return Ok(_scientificResearchFileService.GetScientificResearchFileByType(type));
        }
    }
}
