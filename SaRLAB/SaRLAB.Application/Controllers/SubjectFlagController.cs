using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SubjectDto;
using SaRLAB.DataAccess.Service.SubjectFlagService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectFlagController : Controller
    {
        private readonly ISubjectFlagService _subjectFlagService;
        public SubjectFlagController(ISubjectFlagService subjectFlagService)
        {
            _subjectFlagService = subjectFlagService;
        }

        [HttpGet]
        [Route("GetByID/{email}")]
        public IActionResult GetByID(String email)
        {
            var subjectFlag = _subjectFlagService.getSubjectFlagByUserEmail(email);
            if (subjectFlag == null)
            {
                return BadRequest("cannot find the subjectFlag");
            }
            else
            {
                return Ok(subjectFlag);
            }
        }

        [HttpPost]
        [Route("Update/{email}")]
        public IActionResult updateSubjectFlag(String email,[FromBody]SubjectFlag sub)
        {
            var subjectFlag = _subjectFlagService.updateSubjectFlag(email,sub);
            if (subjectFlag == null)
            {
                return BadRequest("cannot find the subjectFlag");
            }
            else
            {
                return Ok(subjectFlag);
            }
        }
    }
}
