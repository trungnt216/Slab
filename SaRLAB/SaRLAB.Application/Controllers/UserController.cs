using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;
using SaRLAB.DataAccess.Service.UserService;
using Microsoft.AspNetCore.Authorization;
using SaRLAB.DataAccess.Service.SubjectFlagService;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _loginDto;
        private readonly ISubjectFlagService _subjectFlag;

        public UserController(IUserService loginDto, ISubjectFlagService subjectFlag)
        {
            _loginDto = loginDto;
            _subjectFlag = subjectFlag;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_loginDto.GetAll());
        }

        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetAllUser()
        {
            return Ok(_loginDto.GetAllUser());
        }

        [HttpGet]
        [Route("login/{email}/{passWord}")]
        public IActionResult LogIn(string email, string passWord)
        {
            var user = _loginDto.LogIn(email, passWord);

            if (user != null)
            {
                // Return a successful response with the user object
                return Ok(user);
            }
            else
            {
                // If user is not found, return a 404 Not Found response
                return NotFound();
            }
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult LogOut()
        {
            return Ok(new { message = "Logout successful" });
        }


        [HttpPost]
        [Route("update")]
        public IActionResult Update(User user)
        {
            var _user = _loginDto.Update(user);

            if (_user == null)
            {
                return BadRequest("Find the user error"); ;
            }
            else
            {
                return Ok(_user);
            }
        }

        [HttpPost]
        [Route("forgotpassword")]
        public IActionResult ForgotPassword([FromBody] User user)
        {
            var _user = _loginDto.ForgotPassword(user);


            if (_user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_user);
            }
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            if (newUser == null)
            {
                return BadRequest("Invalid data");
            }

            var result = _loginDto.Register(newUser);
            var resultParam = _subjectFlag.InsertSubjectFlag(newUser.Email);
            if (result != null)
            {
                // Return a successful response with the new user object
                return Ok(result);
            }
            else
            {
                // If user already exists, return a 400 Bad Request response
                return BadRequest("User already exists");
            }
        }

        [HttpGet]
        [Route("GetByID/{email}")]
        public IActionResult GetByID(string email)
        {
            if (email == null)
            {
                return BadRequest("Invalid data");
            }

            var user = _loginDto.GetByID(email);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpGet]
        [Route("GetByID_ID/{id}")]
        public IActionResult GetByID_ID(int id)
        {

            var user = _loginDto.GetByID_ID(id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(user);
            }
        }

        [HttpDelete]
        [Route("DeleteById/{id}")]
        public IActionResult DeleteById(int id)
        {
            _loginDto.DeleteById(id);
            return Ok("User deleted successfully.");
        }

        [HttpDelete]
        [Route("DeleteByIds")]
        public IActionResult DeleteByIds([FromBody] string userIds)
        {
            if (string.IsNullOrEmpty(userIds))
            {
                return BadRequest("User IDs are required.");
            }

            _loginDto.DeleteByIds(userIds);
            return Ok("Users deleted successfully.");
        }

        [HttpGet]
        [Route("SearchUser")]
        public IActionResult SearchUsers(string? name, string? email, int? roleId)
        {
            return Ok(_loginDto.SearchUsers(name, email, roleId));
        }


        //lấy toàn bộ user có schoolID, subjectID được nhập vào và có role là Admin
        [HttpGet]
        [Route("GetAllAdminUser/{schoolId}")]
        public IActionResult GetAllAdminUser(int schoolId)
        {
            return Ok(_loginDto.GetUsersByRole(1, schoolId, 0));
        }

        //lấy toàn bộ user có schoolID, subjectID được nhập vào và có role là Teacher
        [HttpGet]
        [Route("GetAllTeacherUser/{schoolId}/{subjectId}")]
        public IActionResult GetAllTeacherUser(int schoolId, int subjectId)
        {
            return Ok(_loginDto.GetUsersByRole(3, schoolId, subjectId));
        }

        //lấy toàn bộ user có schoolID, subjectID được nhập vào và có role là Technical
        [HttpGet]
        [Route("GetAllTechnicalUser/{schoolId}/{subjectId}")]
        public IActionResult GetAllTechnicalUser(int schoolId, int subjectId)
        {
            return Ok(_loginDto.GetUsersByRole(4, schoolId, subjectId));
        }

        //lấy toàn bộ user có schoolID được nhập vào
        [HttpGet]
        [Route("GetAllUserInSchool/{schoolId}")]
        public IActionResult GetAllUserInSchool(int schoolId)
        {
            return Ok(_loginDto.GetAllUserInSchool(schoolId));
        }

        //lấy toàn bộ user có schoolID được nhập vào có role = 5
        [HttpGet]
        [Route("GetAllUserInSchoolRoleUser/{schoolId}")]
        public IActionResult GetAllUserInSchoolRoleUser(int schoolId)
        {
            return Ok(_loginDto.GetUsersByRole(5, schoolId, 0));
        }
    }
}
