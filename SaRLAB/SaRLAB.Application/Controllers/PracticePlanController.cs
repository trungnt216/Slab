using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.PracticePlanService;
using SaRLAB.Models.Entity;
using System.Security.AccessControl;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracticePlanController : Controller
    {
        private readonly IPracticePlanService _practicePlanService;

        public PracticePlanController(IPracticePlanService practicePlanService)
        {
            _practicePlanService = practicePlanService;
        }

        [HttpGet]
        [Route("GetByPracticePlan")]
        public IActionResult GetPracticePlanList([FromBody] PracticePlan practicePlan)
        {
            return Ok(_practicePlanService.GetPracticePlanList(practicePlan));
        }

        [HttpGet]
        [Route("GetByID/{id}")]
        public IActionResult GetPracticePlanById(int id)
        {
            return Ok(_practicePlanService.GetPracticePlanById(id));
        }

        [HttpPost]
        [Route("Update/{id}")]
        public IActionResult UpdatePracticePlanById(int id, [FromBody] PracticePlan practicePlan)
        {
            return Ok(_practicePlanService.UpdatePracticePlanById(id,practicePlan));
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult InsertPracticePlan([FromBody] PracticePlan practicePlan)
        {
            return Ok(_practicePlanService.InsertPracticePlan(practicePlan));
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeletePracticePlanById(int id)
        {
            return Ok(_practicePlanService.DeletePracticePlanById(id));
        }
    }
}
