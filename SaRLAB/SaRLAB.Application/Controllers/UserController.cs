using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;
using SaRLAB.DataAccess.Service.UserService;
using Microsoft.AspNetCore.Authorization;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _loginDto;

        public UserController(IUserService loginDto)
        {
            _loginDto = loginDto;
        }

        [HttpGet]
        [Route("GetAll")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(_loginDto.GetAll());
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
            if(email == null)
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

    }
}
