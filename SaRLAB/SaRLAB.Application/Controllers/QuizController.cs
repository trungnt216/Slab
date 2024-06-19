

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SaRLAB.DataAccess.Service.QuizService;
using SaRLAB.DataAccess.Service.UserService;
using SaRLAB.Models.Entity;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        private readonly IUserService _userService;

        public QuizController(IQuizService quizService, IUserService userService)
        {
            _quizService = quizService;
            _userService = userService;
        }

        [HttpGet]
        [Route("GetRandomQuizzes/{schoolId}/{subjectId}")]
        public IActionResult GetQuizzes(int schoolId, int subjectId)
        {
            return Ok(_quizService.GetRandomQuizzes(50, schoolId, subjectId));
        }

        [HttpGet]
        [Route("GetRandomQuizzesAfterDone/{schoolId}/{subjectId}/{count}")]
        public IActionResult GetQuizzesAfterDone(int count,int schoolId, int subjectId)
        {
            return Ok(_quizService.GetRandomQuizzesAfter(count, schoolId, subjectId));
        }

        [HttpGet]
        [Route("GetQuizById/{id}")]
        public IActionResult GetQuizById(int id)
        {
            return Ok(_quizService.GetQuizById(id));
        }


        [HttpPost]
        [Route("Insert")]
        public IActionResult Insert(Quiz quiz)
        {
            if (quiz == null)
            {
                return BadRequest("not have quiz");
            }
            else
            {
                return Ok(_quizService.InsertQuiz(quiz));
            }
        }

        [HttpPost]
        [Route("Update/{id}")]
        public IActionResult Update(int id, Quiz quiz)
        {
            if (id == 0)
            {
                return BadRequest("not have id");
            }
            else
            {
                return Ok(_quizService.UpdateQuizById(id,quiz));
            }
        }

        [HttpPost]
        [Route("Delete/{ids}")]
        public ActionResult Delete(String ids)
        {
            if (ids == null)
            {
                return BadRequest("not have ids");
            }
            else
            {
                return Ok(_quizService.DeleteQuizByIds(ids));
            }
        }

    }
}
