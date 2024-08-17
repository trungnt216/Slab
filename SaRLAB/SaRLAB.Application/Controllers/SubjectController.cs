using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SubjectDto;
using SaRLAB.Models;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : Controller
    {
        private readonly ISubjectDto _subjectDto;

        public SubjectController(ISubjectDto subjectDto)
        {
            _subjectDto = subjectDto;
        }

        [HttpGet]
        [Route("GetAll")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAll()
        {
            return Ok(_subjectDto.GetAll());
        }

        [HttpGet]
        [Route("GetSubjectBySchool/{schoolId}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetSubjectBySchool(int schoolId)
        {
            return Ok(_subjectDto.GetSubjectBySchool(schoolId));
        }

        [HttpGet]
        [Route("GetSubjectBySchoolAndType/{schoolId}/{id}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetSubjectBySchoolAndType(int schoolId, int id) {
            var subject = _subjectDto.GetSubjectBySchoolAndType(schoolId,id);
            if (subject == null)
            {
                return BadRequest("cannot find the subject");
            }
            else
            {
                return Ok(subject);
            }
        }

        [HttpGet]
        [Route("GetByTypeSchool/{type}/{schoolId}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetByID(int schoolId, int type)
        {
            var subject = _subjectDto.GetByTypeSchool(type,schoolId);
            if (subject == null)
            {
                return BadRequest("cannot find the subject");
            }
            else
            {
                return Ok(subject);
            }
        }

        [HttpGet]
        [Route("GetByName/{name}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetByName(string name)
        {
            var subject = _subjectDto.GetByName(name);
            if (subject == null)
            {
                return BadRequest("cannot find the subject");
            }
            else
            {
                return Ok(subject);
            }
        }

        [HttpPost]
        [Route("Insert")]
        // [Authorize(Roles = "Admin,Owner")]
        public IActionResult Insert(Subject newsubject)
        {
            var _subject = _subjectDto.Insert(newsubject);

            if (_subject == null)
            {
                return BadRequest("Find the user error"); ;
            }
            else
            {
                return Ok(_subject);
            }
        }

        [HttpPost]
        [Route("update")]
        // [Authorize(Roles = "Admin,Owner")]
        public IActionResult Update(Subject newsubject)
        {
            var _subject = _subjectDto.Update(newsubject);

            if (_subject == null)
            {
                return BadRequest("Find the user error"); ;
            }
            else
            {
                return Ok(_subject);
            }
        }
    }
}
