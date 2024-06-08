using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.SubjectDto;
using SaRLAB.DataAccess.Service.SubjectFlagService;

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
                return BadRequest("cannot find the subject");
            }
            else
            {
                return Ok(subjectFlag);
            }
        }
    }
}
