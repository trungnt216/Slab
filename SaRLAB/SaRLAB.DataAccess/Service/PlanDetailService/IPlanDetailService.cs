using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.PlanDetailService
{
    public interface IPlanDetailService
    {
        List<PlanDetail> GetPlanDetailListByPracticePlanId(PlanDetail planDetail);
        PlanDetail GetPlanDetailById(int id);
        int UpdatePlanDetailById(int id, PlanDetail PlanDetail);
        int DeletePlanDetailById(int id);
    }
}
