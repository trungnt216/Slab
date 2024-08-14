using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ReferenceService
{
    public  interface IReferenceService
    {
        List<Reference> GetReferencesBySchoolId(int schoolId);
        Reference GetReferenceById(int id);
        List<Reference>  GetReferenceByName(string name);
        int InsertReference(Reference reference);
        int UpdateReferenceById(int id, Reference reference);
        int DeleteReferenceById(int id);
        List<Reference> GetReferenceByType(string type,int schoolID);

    }
}
