using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SchoolService;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : Controller
    {
        private readonly ISchoolService _schoolService;


        [HttpGet]
        [Route("GetAllSchool")]
        public IActionResult GetAll()
        {
            return Ok(_schoolService.GetAllSchool);
        }

        [HttpGet]
        [Route("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var school = _schoolService.GetSchoolById(id);
            if (school == null)
            {
                return BadRequest("cannot find the school");
            }
            else
            {
                return Ok(school);
            }
        }

        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(School school)
        {
            if (school == null)
            {
                return BadRequest("not have equipment");
            }
            else
            {
                return Ok(_schoolService.InsertSchool);
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_schoolService.DeleteSchoolById(id));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        public IActionResult Update(int id, School school)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_schoolService.UpdateSchoolById(id, school));
            }
        }
    }
}
