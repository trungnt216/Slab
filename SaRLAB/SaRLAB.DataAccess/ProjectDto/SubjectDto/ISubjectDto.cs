using SaRLAB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.ProjectDto.SubjectDto
{
    public interface ISubjectDto
    {
        List<Subject> GetAll();
        Subject GetByID(int id);
        Subject GetByName(string name);
        Subject update(Subject subject);
        Subject insert(Subject subject);
        Subject delete(Subject subject); 
    }
}
