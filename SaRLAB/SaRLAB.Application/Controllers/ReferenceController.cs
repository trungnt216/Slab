using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.DocumentService;
using SaRLAB.DataAccess.Service.ReferenceService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferenceController : Controller
    {
        private readonly IReferenceService _referenceService;

        public ReferenceController(IReferenceService referenceService)
        {
            _referenceService = referenceService;
        }

        [HttpPost]
        [Route("Delete/{id}")]
       // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_referenceService.DeleteReferenceById(id));
            }
        }

        [HttpGet]
        [Route("GetById/{id}")]
        //[Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetById(int id)
        {
            return Ok(_referenceService.GetReferenceById(id));
        }

        [HttpPost]
        [Route("Insert")]
       // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Insert(Reference reference)
        {
            if (reference == null)
            {
                return BadRequest("not have equipment");
            }
            else
            {
                return Ok(_referenceService.InsertReference(reference));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
       // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Update(int id, Reference reference)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_referenceService.UpdateReferenceById(id, reference));
            }
        }

        [HttpGet]
        [Route("GetReferenceBySchoolId/{schoolId}")]
      //  [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetReferenceBySchoolId(int schoolId)
        {
            return Ok(_referenceService.GetReferencesBySchoolId(schoolId));
        }

        [HttpGet]
        [Route("GetReferenceByName/{name}")]
        //  [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetReferenceByName(string name)
        {
            return Ok(_referenceService.GetReferenceByName(name));
        }

    }
    

}
