using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ScientificResearchService
{
    public interface IScientificResearchService
    {
        List<ScientificResearch> GetAll();
    }
}
