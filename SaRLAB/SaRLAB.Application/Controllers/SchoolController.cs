using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SchoolService;
using SaRLAB.DataAccess.Service.SubjectDto;
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
        private readonly ISubjectDto _subjectService;

        public SchoolController(ISchoolService schoolService, IUserService userService, ISubjectDto subjectDto)
        {
            _schoolService = schoolService;
            _userService = userService;
            _subjectService = subjectDto;
        }


        [HttpGet]
        [Route("GetAllSchool")]
        public IActionResult GetAll()
        {
            return Ok(_schoolService.GetAllSchool());
        }

        [HttpGet]
        [Route("GetByID/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
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

        [HttpGet]
        [Route("max")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetBydstID()
        {

            {
                return Ok(_schoolService.GetSchoolWithMaxId());
            }
        }

        [HttpPost]
        [Route("Insert")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Insert(School school)
        {
            if (school == null)
            {
                return BadRequest("School cannot be null");
            }
            var insertedSchool = _schoolService.InsertSchool(school);

            if (insertedSchool == null)
            {
                return BadRequest("Failed to insert school");
            }
            int maxId = 0;
            School schoolMaxId = _schoolService.GetSchoolWithMaxId();
            if(schoolMaxId != null)
            {
                maxId = schoolMaxId.ID.GetValueOrDefault();
            } else
            {
                maxId = 1;
            }
            List<Subject> subjects = new List<Subject>();
            for (int i = 1; i <= 32; i++)
            {
                subjects.Add(new Subject
                {
                    SubjectName = "B" + i,
                    Rule = "Rule for subject " + i,
                    SchoolId = maxId, // Use the generated school ID
                    Type = i
                });
            }
            _subjectService.InsertSubjects(subjects);
            return Ok(insertedSchool);
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
                var result = _schoolService.DeleteSchoolById(id);
                var resultParam = _userService.DeleteBySchoolId(id);

                return Ok(result);
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
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
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
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
