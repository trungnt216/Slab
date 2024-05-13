using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ScientificResearchService
{
    public class ScientificResearchService : IScientificResearchService
    {
        private readonly ApplicationDbContext _context;

        public ScientificResearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ScientificResearch> GetAll()
        {
            var scientificResearch = _context.scientificResearches.Select(x => new ScientificResearch
            {
                Author = x.Author,
                CreateBy = x.CreateBy,
                CreateTime = x.CreateTime,
                ID = x.ID,
                Name = x.Name,
                PublicationDate = x.PublicationDate,
                UpdateBy = x.UpdateBy,
                UpdateTime = x.UpdateTime,
            });
            return scientificResearch.ToList();
        }
    }
}
