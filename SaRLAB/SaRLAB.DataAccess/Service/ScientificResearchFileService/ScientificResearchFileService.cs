using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ScientificResearchFileService
{
    public class ScientificResearchFileService : IScientificResearchFileService
    {
        private readonly ApplicationDbContext _context;

        public ScientificResearchFileService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteScientificResearchFileById(int id)
        {
            var fileToDelete = _context.ScientificResearchFiles.Find(id);
            if (fileToDelete != null)
            {
                _context.ScientificResearchFiles.Remove(fileToDelete);
                return _context.SaveChanges();
            }
            return 0;
        }

        public List<ScientificResearchFile> GetFilesByScientificResearchId(int id)
        {
            return _context.ScientificResearchFiles
              .Where(file => file.ResearchFileID== id)
              .ToList();
        }

        public ScientificResearchFile GetScientificResearchFileId(int id)
        {
            var ScientificResearchFile = _context.ScientificResearchFiles.SingleOrDefault(item => (item.ID == id));

            if (ScientificResearchFile == null)
            {
                return null;
            }
            else
            {
                return ScientificResearchFile;
            }
        }

        public int InsertScientificResearchFile(ScientificResearchFile scientificResearchFile)
        {
            _context.ScientificResearchFiles.Add(scientificResearchFile);
            return _context.SaveChanges();
        }

        public int UpdateScientificResearchFileById(int id, ScientificResearchFile scientificResearchFile)
        {
            var existingFile = _context.ScientificResearchFiles.Find(id);
            if (existingFile != null)
            {
                existingFile.Path = scientificResearchFile.Path;
                existingFile.Type = scientificResearchFile.Type;
                existingFile.CreateBy = scientificResearchFile.CreateBy;
                existingFile.CreateTime = scientificResearchFile.CreateTime;
                existingFile.UpdateBy = scientificResearchFile.UpdateBy;
                existingFile.UpdateTime = scientificResearchFile.UpdateTime;
                existingFile.ResearchFileID = scientificResearchFile.ResearchFileID;
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
