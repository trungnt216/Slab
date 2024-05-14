using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ScientificResearchFileService
{
    public interface IScientificResearchFileService
    {
        List<ScientificResearchFile> GetFilesByScientificResearchId(int ScientificResearchId);
        ScientificResearchFile GetScientificResearchFileId(int id);
        int InsertScientificResearchFile(ScientificResearchFile scientificResearchFile);
        int UpdateScientificResearchFileById(int id, ScientificResearchFile scientificResearchFile);
        int DeleteScientificResearchFileById(int id);

    }
}
