using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.ScientificResearchService;

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
            return Ok(_scientificResearchService.GetAll());
        }

        [HttpGet]
        [Route("{subject}/GetAll")]
        public IActionResult GetAllBySubject(string subject)
        {
            return Ok();
        }
    }
}
