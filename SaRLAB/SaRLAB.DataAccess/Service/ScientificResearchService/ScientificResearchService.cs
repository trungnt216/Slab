using SaRLAB.DataAccess.Service.RoleManageService;
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
        public int DeleteScientificResearchById(int id)
        {
            var researchToDelete = _context.ScientificResearchs.Find(id);
            if (researchToDelete != null)
            {
                _context.ScientificResearchs.Remove(researchToDelete);
                return _context.SaveChanges();
            }
            return 0;
        }

        public List<ScientificResearch> GetScientificResearchsBySubjectId(int subjectId)
        {
            return _context.ScientificResearchs
                            .Where(r => r.SubjectId == subjectId)
                            .ToList();
        }

        public int InsertScientificResearch(ScientificResearch scientificResearch)
        {
            _context.ScientificResearchs.Add(scientificResearch);
            return _context.SaveChanges();
        }

        public int UpdateScientificResearchById(int id, ScientificResearch updatedResearch)
        {
            var existingResearch = _context.ScientificResearchs.Find(id);
            if (existingResearch != null)
            {
                // Cập nhật thông tin của nghiên cứu hiện có với thông tin mới

                existingResearch.Name = updatedResearch.Name ?? existingResearch.Name;
                existingResearch.Author = updatedResearch.Author ?? existingResearch.Author;
                existingResearch.CreateBy = updatedResearch.CreateBy ?? existingResearch.CreateBy;
                existingResearch.remark = updatedResearch.remark ?? existingResearch.remark;
                existingResearch.CreateTime = updatedResearch.CreateTime ?? existingResearch.CreateTime;
                existingResearch.UpdateBy = updatedResearch.UpdateBy ?? existingResearch.UpdateBy;
                existingResearch.UpdateTime = updatedResearch.UpdateTime ?? existingResearch.UpdateTime;
                existingResearch.PublicationDate = updatedResearch.PublicationDate ?? existingResearch.PublicationDate;
                existingResearch.AdminVerifyFlag = updatedResearch.AdminVerifyFlag ?? existingResearch.AdminVerifyFlag;
                existingResearch.UserVerifyFlag = updatedResearch.UserVerifyFlag ?? existingResearch.UserVerifyFlag;

                // Cập nhật thông tin trong context và trả về số lượng hàng bị ảnh hưởng
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
