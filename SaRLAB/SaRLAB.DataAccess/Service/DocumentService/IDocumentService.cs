using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.DocumentService
{
    public interface IDocumentService
    {
        List<Document> GetDocumentsBySubjectId (int subjectId);
        Document GetDocumentById (int id);
        int InsertDocument(Document document);

        int UpdateDocumentById(int id,Document document);
        int DeleteDocumentById(int id);

        List<Document> getNormalDocument();
        List<Document> getPageDocument();
        List<Document> getSpecializedEnglishDocument();
    }
}
