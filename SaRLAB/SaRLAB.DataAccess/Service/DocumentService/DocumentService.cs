using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SaRLAB.DataAccess.Service.DocumentService
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;

        public DocumentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteDocumentById(int id)
        {
            var documentToDelete = _context.Documents.Find(id);
            if (documentToDelete == null)
                return 0;

            _context.Documents.Remove(documentToDelete);
            return _context.SaveChanges();
        }


        public Document GetDocumentById(int id)
        {
            var document = _context.Documents.SingleOrDefault(item => (item.ID == id));

            if (document == null)
            {
                return null;
            }
            else
            {
                return document;
            }
        }

        public List<Document> GetDocumentsBySubjectId(int subjectId)
        {
            return _context.Documents.Where(e => e.SubjectId == subjectId).ToList();
        }

        public List<Document> GetDocumentsByType(int schoolId, int subjectId, string type)
        {
                return _context.Documents
                    .Where(d => d.SchoolId == schoolId && d.SubjectId == subjectId && d.Type == type)
                   .ToList();
        }

        public List<Document> getNormalDocument()
        {
                return _context.Documents
                    .Where(doc => doc.SpecializedEnglishFlag == false && doc.PageFlag == false)
                    .ToList();
        }

        public List<Document> getPageDocument()
        {
            {
                return _context.Documents
                    .Where(doc => doc.SpecializedEnglishFlag == false && doc.PageFlag == true)
                    .ToList();
            }
        }

        public List<Document> getSpecializedEnglishDocument()
        {
            {
                return _context.Documents
                    .Where(doc => doc.SpecializedEnglishFlag == true && doc.PageFlag == false)
                    .ToList();
            }
        }

        public int InsertDocument(Document document)
        {
            _context.Documents.Add(document);
            return _context.SaveChanges();
        }

        public int UpdateDocumentById(int id, Document document)
        {
            var existingDocument = _context.Documents.FirstOrDefault(d => d.ID == id);

            if (existingDocument == null)
            {
                // Document with the specified ID does not exist
                return 0; // Or you can throw an exception or handle it based on your requirement
            }

            // Update the properties of the existing document with the values from the provided document
            existingDocument.Name = document.Name ?? existingDocument.Name;
            existingDocument.Path = document.Path ?? existingDocument.Path;
            existingDocument.SpecializedEnglishFlag = document.SpecializedEnglishFlag ?? existingDocument.SpecializedEnglishFlag;
            existingDocument.PageFlag = document.PageFlag ?? existingDocument.PageFlag;
            existingDocument.UpdateBy = document.UpdateBy ?? existingDocument.UpdateBy;
            existingDocument.UpdateTime = document.UpdateTime ?? existingDocument.UpdateTime;
            existingDocument.SubjectId = document.SubjectId; // Assuming SubjectId should always be updated

            // Save changes to the database
            _context.SaveChanges();

            return 1; // Or you can return the ID of the updated document, or any other meaningful value
        }
    }
}
