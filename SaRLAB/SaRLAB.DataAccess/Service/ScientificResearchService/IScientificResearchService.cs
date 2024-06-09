using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ScientificResearchService
{
    public interface  IScientificResearchService
    {
        List<ScientificResearch> GetScientificResearchsBySubjectId(int subjectId);
        int InsertScientificResearch(ScientificResearch scientificResearch);
        int UpdateScientificResearchById(int id, ScientificResearch scientificResearch);
        int DeleteScientificResearchById(int id);
        ScientificResearch GetScientificResearchById(int id);
    }
}
