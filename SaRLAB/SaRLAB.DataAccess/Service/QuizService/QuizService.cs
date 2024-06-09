using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.QuizService
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext _context;

        public QuizService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteQuizById(int id)
        {
            var quiz = _context.Quizzes.Find(id);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                return _context.SaveChanges();
            }
            return 0;
        }

        public int DeleteQuizByIds(string ids)
        {
            var idList = ids.Split(',').Select(int.Parse).ToList();

            // Tìm và xóa các câu hỏi dựa trên danh sách id
            var quizzes = _context.Quizzes.Where(q => idList.Contains(q.ID.Value)).ToList();
            _context.Quizzes.RemoveRange(quizzes);

            // Lưu thay đổi vào cơ sở dữ liệu và trả về số lượng bản ghi bị ảnh hưởng
            return _context.SaveChanges();
        }


        public Quiz GetQuizById(int id)
        {
            return _context.Quizzes.Find(id);
        }

        public List<Quiz> GetQuizzesAccordingSchoolAndSubject(int schoolId, int subjectId)
        {
            return _context.Quizzes.Where(q => q.SchoolId == schoolId && q.SubjectId == subjectId).ToList();
        }

        public int InsertQuiz(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            return _context.SaveChanges();
        }

        public int UpdateQuizById(int id, Quiz quiz)
        {
            var existingQuiz = _context.Quizzes.Find(id);
            if (existingQuiz != null)
            {
                existingQuiz.Question = quiz.Question;
                existingQuiz.QuestionImage = quiz.QuestionImage;
                existingQuiz.OptionA = quiz.OptionA;
                existingQuiz.OptionAImage = quiz.OptionAImage;
                existingQuiz.OptionB = quiz.OptionB;
                existingQuiz.OptionBImage = quiz.OptionBImage;
                existingQuiz.OptionC = quiz.OptionC;
                existingQuiz.OptionCImage = quiz.OptionCImage;
                existingQuiz.OptionD = quiz.OptionD;
                existingQuiz.OptionDImage = quiz.OptionDImage;
                existingQuiz.CorrectAnswer = quiz.CorrectAnswer;
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
