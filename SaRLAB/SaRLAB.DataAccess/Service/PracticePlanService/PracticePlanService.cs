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

        public List<PracticePlan> GetPracticePlanAccordingtoProgramList()
        {
            return _context.PracticePlans
                .Where(plan => plan.PracticeType.Equals("1"))
                .ToList();
        }

        public PracticePlan GetPracticePlanById(int id)
        {
            return _context.PracticePlans.Find(id);
        }

        public List<PracticePlan> GetPracticePlanResearchList()
        {
            return _context.PracticePlans
                .Where(plan => plan.PracticeType.Equals("2"))
                .ToList();
        }

        public int InsertPracticePlan(PracticePlan practicePlan)
        {
            // Check if a practice plan with the same name already exists
            var existingPlan = _context.PracticePlans.FirstOrDefault(pp => pp.Name == practicePlan.Name);
            if (existingPlan != null)
            {
                // Return a status code or throw an exception indicating that the plan already exists
                return -1; // Or throw new Exception("Practice plan with the same name already exists.");
            }

            // Add the new practice plan to the context
            _context.PracticePlans.Add(practicePlan);

            // Save changes to the database
            return _context.SaveChanges();
        }

        public List<PracticePlan> SearchPracticePlan(string name)
        {
            IQueryable<PracticePlan> query = _context.PracticePlans;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name.Contains(name));
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

                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
