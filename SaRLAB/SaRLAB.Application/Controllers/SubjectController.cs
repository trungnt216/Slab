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
        public IActionResult GetAll()
        {
            return Ok(_subjectDto.GetAll());
        }

        [HttpGet]
        [Route("GetByID/{id}")]
        public IActionResult GetByID(int id) {
            var subject = _subjectDto.GetByID(id);
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
