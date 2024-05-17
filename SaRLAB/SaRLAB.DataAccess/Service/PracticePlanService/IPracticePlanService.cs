using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.PracticePlanService
{
    public interface IPracticePlanService
    {
        List<PracticePlan> GetPracticePlanListByPracticePlanId(int id);
        PracticePlan GetPracticePlanById(int id);
        int UpdatePracticePlanById(int id, PracticePlan practicePlan);
        int DeletePracticePlanById(int id);

    }
}
