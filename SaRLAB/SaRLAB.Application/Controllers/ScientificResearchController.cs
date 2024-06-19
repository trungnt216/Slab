using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.ScientificResearchService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScientificResearchController : Controller
    {
        private readonly IScientificResearchService _scientificResearchService;
        public ScientificResearchController(IScientificResearchService scientificResearchService)
        {
            _scientificResearchService = scientificResearchService;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            return Ok();
        }

        [HttpGet]
        [Route("{subjectId}/GetAll")]
        public IActionResult GetAllBySubject(int subjectId)
        {
            return Ok(_scientificResearchService.GetScientificResearchsBySubjectId(subjectId));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_scientificResearchService.GetScientificResearchById(id));
        }

        [HttpPost]
        [Route("Update/{subjectId}")]
        public IActionResult UpdateBySubject(int subjectId, ScientificResearch updatedResearch)
        {
            if(subjectId == null)
            {
                return BadRequest("Error to search subject");
            }
            else
            {
                return Ok(_scientificResearchService.UpdateScientificResearchById(subjectId, updatedResearch));
            }
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult InsertBySubject(ScientificResearch updatedResearch)
        {
            if (updatedResearch == null)
            {
                return BadRequest("Error to search subject");
            }
            else
            {
                return Ok(_scientificResearchService.InsertScientificResearch(updatedResearch));
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public IActionResult DeleteBySubject(int id)
        {
            if (id == null)
            {
                return BadRequest("Error to search subject");
            }
            else
            {
                return Ok(_scientificResearchService.DeleteScientificResearchById(id));
            }
        }
    }
}
