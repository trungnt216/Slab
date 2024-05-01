﻿using SaRLAB.Models;
using System.Net.Mail;
using System.Net;
using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;

namespace SaRLAB.DataAccess.ProjectDto.LoginDto
{
    public class LoginDto : ILoginDto
    {
        private readonly ApplicationDbContext _context;

        public LoginDto(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<User> GetAll()
        {
            var user = _context.User.Select(value => new User
            {
                Name = value.Name,
            });
            return user.ToList();
        }

        public User LogIn(string email, string passWord)
        {
            var user = _context.User.SingleOrDefault(item => (item.Email == email && item.Password == passWord));

            User userlogin = new User
            {
                Name = user.Name,
                Password = user.Password,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                DateOfBirth = user.DateOfBirth,
                CreateBy = user.CreateBy,
                UpdateBy = user.UpdateBy,
                CreateTime = user.CreateTime,
            };

            return userlogin;
        }

        public User LogOut(string email, string passWord)
        {
            throw new NotImplementedException();
        }


        public User Update(User user)
        {
            var _user = _context.User.SingleOrDefault(item =>(item.Email == user.Email));

            if (_user != null)
            {
                _user.Password = user.Password;
                _user.Name = user.Name;
                _user.DateOfBirth = user.DateOfBirth;
                _user.CreateBy = user.CreateBy;
                _user.UpdateBy = user.UpdateBy;
                _user.CreateTime = DateTime.Now;
                _user.Role = user.Role;
                _context.SaveChanges();
            }


            return _user;
        }

        public User ForgotPassword(User user)
        {
            var _user = _context.User.SingleOrDefault(item => (item.Email == user.Email));

            if (_user != null)
            {
                string newPassword = GenerateRandomPassword();
                _user.Password = newPassword;
                _context.SaveChanges();
                _ = SendEmail(_user.Email, _user.Password);
            }

            return _user;
        }

        private string GenerateRandomPassword()
        {
            // Generate a random password using your preferred method
            // For example:
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] password = new char[8];
            for (int i = 0; i < password.Length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            return new string(password);
        }

        private async Task SendEmail(string toAddress, string newPassword)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("your-email@gmail.com");
                mail.To.Add(toAddress);
                mail.Subject = "Password Reset";
                mail.Body = "Your new password is: " + newPassword;

                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("your-email@gmail.com", "your-password");
                    smtpClient.EnableSsl = true;

                    await smtpClient.SendMailAsync(mail);
                }
            }
        }

        public User Register(User user)
        {
            var existingUser = _context.User.SingleOrDefault(u => ((u.Email == user.Email) || (u.Phone == user.Phone)));

            if (existingUser != null)
            {
                return null;
            }

            var newUser = new User
            {
                Name = user.Name,
                LoginName = user.LoginName,
                Password = user.Password,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth.Date,
                CreateBy = user.CreateBy,
                UpdateBy = user.UpdateBy,
                CreateTime = DateTime.Now,
                Role = "Student",


            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }
    }
}