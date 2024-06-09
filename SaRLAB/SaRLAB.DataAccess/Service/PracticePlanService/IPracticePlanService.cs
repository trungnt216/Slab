using  SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.PracticePlanService
{
    public interface IPracticePlanService
    {
        List<PracticePlan> GetPracticePlanAccordingtoProgramList();
        List<PracticePlan> GetPracticePlanResearchList();
        List<PracticePlan> SearchPracticePlan(String? name);
        PracticePlan GetPracticePlanById(int id);
        int UpdatePracticePlanById(int id, PracticePlan practicePlan);
        int DeletePracticePlanById(int id);
        int InsertPracticePlan(PracticePlan practicePlan);

    }
}
