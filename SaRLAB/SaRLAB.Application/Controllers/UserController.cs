﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;
using SaRLAB.DataAccess.Service.UserDto;

namespace SaRLAB.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDto _loginDto;

        public UserController(IUserDto loginDto)
        {
            _loginDto = loginDto;
        }

        [HttpGet]
        [Route("GetAll")]
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

    }
}
