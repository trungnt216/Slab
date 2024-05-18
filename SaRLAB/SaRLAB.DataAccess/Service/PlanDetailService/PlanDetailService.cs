using  SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.PlanDetailService
{
    public class PlanDetailService : IPlanDetailService
    {
        private readonly ApplicationDbContext _context;

        public PlanDetailService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int DeletePlanDetailById(int id)
        {
            var planDetail = _context.PlanDetails.FirstOrDefault(p => p.ID == id);
            if (planDetail != null)
            {
                _context.PlanDetails.Remove(planDetail);
                return _context.SaveChanges();
            }
            return 0; // Return 0 if plan detail with given ID is not found
        }

        public PlanDetail GetPlanDetailById(int id)
        {
            return _context.PlanDetails.FirstOrDefault(p => p.ID == id);
        }

        public List<PlanDetail> GetPlanDetailListByPracticePlanId(PlanDetail planDetail)
        {
            return _context.PlanDetails.Where(p => p.PracticePlanId == planDetail.PracticePlanId).ToList();
        }

        public int UpdatePlanDetailById(int id, PlanDetail updatedPlanDetail)
        {
            {
                var existingPlanDetail = _context.PlanDetails.FirstOrDefault(p => p.ID == id);
                if (existingPlanDetail != null)
                {
                    // Update properties of existing plan detail with values from updatedPlanDetail
                    existingPlanDetail.EquipmentQuantity = updatedPlanDetail.EquipmentQuantity;
                    existingPlanDetail.UpdateBy = updatedPlanDetail.UpdateBy;
                    existingPlanDetail.UpdateTime = updatedPlanDetail.UpdateTime;

                    // Save changes to the database
                    return _context.SaveChanges();
                }
                return 0; // Return 0 if plan detail with given ID is not found
            }
        }
    }
}
