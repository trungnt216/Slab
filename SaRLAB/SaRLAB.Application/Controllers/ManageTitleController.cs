using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.ManageTitleService;
using SaRLAB.Models.Entity;
namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageTitleController : Controller
    {
        private readonly IManageTitleService _manageService;

        public ManageTitleController(IManageTitleService manageTitleService)
        {
            _manageService = manageTitleService;
        }

        [HttpGet]
        [Route("GetManageTitlesAccordingSchool/{schoolId}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetManageTitlesAccordingSchool(int schoolId)
        {
            return Ok(_manageService.GetManageTitlesAccordingSchool(schoolId));
        }

        [HttpGet]
        [Route("GetTitleBySchoolAnSubject/{schoolId}/{subjectId}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetTitleBySchoolAnSubject(int schoolId, int subjectId)
        {
            return Ok(_manageService.GetManageTitlesAccordingSchoolAndSubject(schoolId,subjectId));
        }

        [HttpGet]
        [Route("GetTitleBySchoolAnSubjectAndType/{schoolId}/{subjectId}/{type}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetTitleBySchoolAnSubjectAndType(int schoolId, int subjectId, int type)
        {
            return Ok(_manageService.GetManageTitleAccordingSchoolAndSubjectAndType(schoolId,subjectId,type));
        }

        [HttpPost]
        [Route("Insert")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Insert(ManageTitle manageTitle)
        {
            if (manageTitle == null)
            {
                return BadRequest("not have quizTitle");
            }
            else
            {
                return Ok(_manageService.InsertManageTitle(manageTitle));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Update(int id, ManageTitle manageTitle)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_manageService.UpdateManageTitleById(id, manageTitle));
            }
        }


        [HttpPost]
        [Route("Delete/{id}")]
        // [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult DeleteById(int id)
        {
            return Ok(_manageService.DeleteManageTitleById(id));
        }
    }
}
