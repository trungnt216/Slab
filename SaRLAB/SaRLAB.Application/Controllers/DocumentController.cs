﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        [Route("getNormalDocument")]
        public IActionResult getNormalDocument()
        {
            return Ok(_documentService.getNormalDocument());
        }

        [HttpGet]
        [Route("getPageDocument")]
        public IActionResult getPageDocument()
        {
            return Ok(_documentService.getPageDocument());
        }

        [HttpGet]
        [Route("getSpecializedEnglishDocument")]
        public IActionResult getSpecializedEnglishDocument()
        {
            return Ok(_documentService.getSpecializedEnglishDocument());
        }


        [HttpGet]
        [Route("GetAllByType/{schoolId}/{subjectId}/{type}")]
        public IActionResult GetAllByType(int schoolId, int subjectId, string type)
        {
            return Ok(_documentService.GetDocumentsByType(schoolId, subjectId, type));
        }

        [HttpGet]
        [Route("GetAllByTypeToAccept/{schoolId}/{subjectId}/{type}")]
        public IActionResult GetAllByTypeToAccept(int schoolId, int subjectId, string type)
        {
            return Ok(_documentService.GetDocumentsByTypeToAccept(schoolId, subjectId, type));
        }


        //get all document in the school id and check the flag if the document is admin accept to display
        [HttpGet]
        [Route("GetAllDocumentBySchoolId/{schoolId}")]
        public IActionResult GetAllByType(int schoolId)
        {
            return Ok(_documentService.GetDocumentsBySchool(schoolId));
        }
    }
}
