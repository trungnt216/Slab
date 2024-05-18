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
                return Ok(_documentService.DeleteDocumentById(id));
            }
        }

        [HttpGet]
        [Route("GetBySubject/{id}")]
        public IActionResult GetAllBySubject(int id)
        {
            return Ok(_documentService.GetDocumentsBySubjectId(id));
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            return Ok(_documentService.GetDocumentById(id));
        }

        [HttpPost]
        [Route("Insert")]
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
    }
}
