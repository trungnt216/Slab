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

        SubjectFlag getSubjectFlagByUserId(int userId);
        int InsertSubjectFlag(SubjectFlag subjectFlag);
        int updateSubjectFlag(int userId, SubjectFlag subjectFlag);
    }
}
