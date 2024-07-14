using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ManageTitleService
{
    public interface IManageTitleService
    {
        List<ManageTitle> GetManageTitlesAccordingSchoolAndSubject(int schoolId, int subjectId);
        ManageTitle GetManageTitleAccordingSchoolAndSubjectAndType(int schoolId, int subjectId, int Type);
        int InsertManageTitle(ManageTitle manageTitle);
        int UpdateManageTitleById(int id, ManageTitle manageTitle);
        int DeleteManageTitleById(int id);
    }
}
