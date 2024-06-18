using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SchoolService;
using SaRLAB.DataAccess.Service.UserService;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : Controller
    {
        private readonly ISchoolService _schoolService;
        private readonly IUserService _userService;

        public SchoolController(ISchoolService schoolService, IUserService userService)
        {
            _schoolService = schoolService;
            _userService = userService;
        }


        [HttpGet]
        [Route("GetAllSchool")]
        public IActionResult GetAll()
        {
            return Ok(_schoolService.GetAllSchool());
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
                return Ok(_schoolService.InsertSchool(school));
            }
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
                var result = _schoolService.DeleteSchoolById(id);
                var resultParam = _userService.DeleteBySchoolId(id);

                return Ok(result);
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


        [HttpPost]
        [Route("RecoverSchool/{id}")]
        public IActionResult RecoverSchool(int id)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_schoolService.RecoverSchool(id));
            }
        }
    }
}
