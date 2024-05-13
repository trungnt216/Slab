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

        [HttpPost]
        [Route("{subjectId}/Update")]
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
        [Route("{subjectId}/Insert")]
        public IActionResult InsertBySubject(int subjectId, ScientificResearch updatedResearch)
        {
            if (subjectId == null && updatedResearch == null)
            {
                return BadRequest("Error to search subject");
            }
            else
            {
                updatedResearch.SubjectId = subjectId;
                return Ok(_scientificResearchService.InsertScientificResearch(updatedResearch));
            }
        }

        [HttpDelete]
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
