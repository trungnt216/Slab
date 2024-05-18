using  SaRLAB.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.PracticePlanService
{
    public class PracticePlanService : IPracticePlanService
    {
        private readonly ApplicationDbContext _context;

        public PracticePlanService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeletePracticePlanById(int id)
        {
            var practicePlanToDelete = _context.PracticePlans
                               .Include(sr => sr.PlanDetails)
                               .FirstOrDefault(sr => sr.ID == id);

            if (practicePlanToDelete != null)
            {
                // Xóa tất cả các thực thể ScientificResearchFile liên quan trước
                _context.PlanDetails.RemoveRange(practicePlanToDelete.PlanDetails);

                // Sau đó xóa thực thể ScientificResearch
                _context.PracticePlans.Remove(practicePlanToDelete);

                return _context.SaveChanges();
            }

            return 0;
        }

        public PracticePlan GetPracticePlanById(int id)
        {
            return _context.PracticePlans.Find(id);
        }

        public List<PracticePlan> GetPracticePlanList(PracticePlan practicePlan)
        {
            IQueryable<PracticePlan> query = _context.PracticePlans;

            if (practicePlan != null)
            {
                if (!string.IsNullOrEmpty(practicePlan.Name))
                    query = query.Where(pp => pp.Name == practicePlan.Name);

                if (!string.IsNullOrEmpty(practicePlan.PracticeType))
                    query = query.Where(pp => pp.PracticeType == practicePlan.PracticeType);


                if (practicePlan.SubjectId != 0)
                    query = query.Where(pp => pp.SubjectId == practicePlan.SubjectId);
            }

            return query.ToList();
        }

        public int UpdatePracticePlanById(int id, PracticePlan updatedPracticePlan)
        {
            var practicePlan = _context.PracticePlans.Find(id);
            if (practicePlan != null)
            {
                // Update properties of the retrieved practicePlan entity
                practicePlan.Name = updatedPracticePlan.Name;
                practicePlan.PracticeType = updatedPracticePlan.PracticeType;   
                practicePlan.UpdateBy = updatedPracticePlan.UpdateBy;
                practicePlan.UpdateTime = updatedPracticePlan.UpdateTime;
                practicePlan.SubjectId = updatedPracticePlan.SubjectId;

                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
