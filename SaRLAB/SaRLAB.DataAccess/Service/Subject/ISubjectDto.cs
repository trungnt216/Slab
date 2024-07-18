using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.SubjectDto
{
    public interface ISubjectDto
    {
        List<Subject> GetAll();
        Subject GetByID(int id);
        Subject GetByTypeSchool(int type, int schoolId);
        Subject GetByName(string name);
        Subject Update(Subject subject);
        Subject Insert(Subject subject);
        void DeleteById(int id);
        Subject GetSubjectBySchoolAndType(int schoolId, int Type);
        List<Subject> GetSubjectBySchool(int schoolId);
        void InsertSubjects(List<Subject> subjects);
    }
}
