using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.SchoolService
{
    public interface ISchoolService
    {
        List<School> GetAllSchool();
        School GetSchoolById(int id);
        int DeleteSchoolById(int id);
        int UpdateSchoolById(int id, School school);
    }
}
