using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.QuizService
{
    public interface IQuizService
    {
        List<Quiz> GetQuizzesAccordingSchoolAndSubject(int schoolId, int subjectId);
        List<Quiz> GetRandomQuizzes(int count, int schoolId, int subjectId);
        List<Quiz> GetRandomQuizzesAfter(int count, int schoolId, int subjectId);
        Quiz GetQuizById(int id);
        int InsertQuiz(Quiz quiz);
        int UpdateQuizById(int id, Quiz quiz);
        int DeleteQuizById(int id);
        int DeleteQuizByIds(string ids);
    }
}
