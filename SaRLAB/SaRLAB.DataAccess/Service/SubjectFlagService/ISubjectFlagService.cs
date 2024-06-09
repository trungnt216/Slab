using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.SubjectFlagService
{
    public  interface ISubjectFlagService
    {

        SubjectFlag getSubjectFlagByUserEmail(String userEmail);
        int InsertSubjectFlag(String userEmail);
        int updateSubjectFlag(String userEmail, SubjectFlag subjectFlag);
    }
}
