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
        Subject GetByName(string name);
        Subject Update(Subject subject);
        Subject Insert(Subject subject);
        Subject Delete(Subject subject); 
    }
}
