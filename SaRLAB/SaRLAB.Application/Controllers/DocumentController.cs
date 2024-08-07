using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.DataAccess.Service.DocumentService;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
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
                return Ok(_documentService.DeleteDocumentById(id));
            }
        }

        [HttpGet]
        [Route("GetBySubject/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAllBySubject(int id)
        {
            return Ok(_documentService.GetDocumentsBySubjectId(id));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetById(int id)
        {
            return Ok(_documentService.GetDocumentById(id));
        }

        [HttpGet]
        [Route("GetByIdDocumentsBySchoolToAccept/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetByIdDocumentsBySchoolToAccept(int id)
        {
            return Ok(_documentService.GetByIdDocumentsBySchoolToAccept(id));
        }

        [HttpPost]
        [Route("Insert")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Insert(Document document)
        {
            if (document == null)
            {
                return BadRequest("not have equipment");
            }
            else
            {
                return Ok(_documentService.InsertDocument(document));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult Update(int id, Document document)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_documentService.UpdateDocumentById(id, document));
            }
        }

        [HttpGet]
        [Route("getNormalDocument")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult getNormalDocument()
        {
            return Ok(_documentService.getNormalDocument());
        }

        [HttpGet]
        [Route("getPageDocument")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult getPageDocument()
        {
            return Ok(_documentService.getPageDocument());
        }

        [HttpGet]
        [Route("getSpecializedEnglishDocument")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult getSpecializedEnglishDocument()
        {
            return Ok(_documentService.getSpecializedEnglishDocument());
        }


        [HttpGet]
        [Route("GetAllByType/{schoolId}/{subjectId}/{type}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAllByType(int schoolId, int subjectId, string type)
        {
            return Ok(_documentService.GetDocumentsByType(schoolId, subjectId, type));
        }

        [HttpGet]
        [Route("GetAllByTypeToAccept/{schoolId}/{subjectId}/{type}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAllByTypeToAccept(int schoolId, int subjectId, string type)
        {
            return Ok(_documentService.GetDocumentsByTypeToAccept(schoolId, subjectId, type));
        }

        [HttpGet]
        [Route("GetAllDocumentsBySchoolToAccept/{schoolId}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAllDocumentsBySchoolToAccept(int schoolId)
        {
            return Ok(_documentService.GetAllDocumentsBySchoolToAccept(schoolId));
        }


        //get all document in the school id and check the flag if the document is admin accept to display
        [HttpGet]
        [Route("GetAllDocumentBySchoolId/{schoolId}")]
        [Authorize(Roles = "Admin,Owner,Teacher,Technical,User")]
        public IActionResult GetAllByType(int schoolId)
        {
            return Ok(_documentService.GetDocumentsBySchool(schoolId));
        }
    }
}
