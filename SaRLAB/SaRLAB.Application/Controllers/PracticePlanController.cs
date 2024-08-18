using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.EquipmentService;
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

        [HttpPost]
        [Route("SearchPracticePlan")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult SearchPracticePlan(String? name)
        {
            return Ok(_practicePlanService.SearchPracticePlan(name));
        }

        [HttpPost]
        [Route("GetPracticePlanAccordingtoProgramList")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetPracticePlanAccordingtoProgramList()
        {
            return Ok(_practicePlanService.GetPracticePlanAccordingtoProgramList());
        }

        [HttpPost]
        [Route("GetPracticePlanResearchList")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetPracticePlanResearchList()
        {
            return Ok(_practicePlanService.GetPracticePlanResearchList());
        }

        [HttpGet]
        [Route("GetByID/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetPracticePlanById(int id)
        {
            return Ok(_practicePlanService.GetPracticePlanById(id));
        }

        [HttpPost]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult UpdatePracticePlanById(int id, [FromBody] PracticePlan practicePlan)
        {
            return Ok(_practicePlanService.UpdatePracticePlanById(id,practicePlan));
        }

        [HttpPost]
        [Route("Insert")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult InsertPracticePlan([FromBody] PracticePlan practicePlan)
        {

            return Ok(_practicePlanService.InsertPracticePlan(practicePlan));
        }

        [HttpPost]
        [Route("Delete/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult DeletePracticePlanById(int id)
        {
            return Ok(_practicePlanService.DeletePracticePlanById(id));
        }
    }
}
