using System.Net;
using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.UserDto
{
    public class UserDto : IUserDto
    {
        private readonly ApplicationDbContext _context;

        public UserDto(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<User> GetAll()
        {
            var user = _context.Users.Select(value => new User
            {
                ID = value.ID,
                Name = value.Name,
                Role_ID = value.Role_ID,
                RoleManages = value.RoleManages
            });
            return user.ToList();
        }

        public User LogIn(string email, string passWord)
        {
            var user = _context.Users.SingleOrDefault(item => (item.Email == email && item.Password == passWord));

            if (user != null)
            {
                User userlogin = new User
                {
                    Name = user.Name,
                    Password = user.Password,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role_ID = user.Role_ID,
                    DateOfBirth = user.DateOfBirth,
                    CreateBy = user.CreateBy,
                    UpdateBy = user.UpdateBy,
                    CreateTime = user.CreateTime,
                };

                return userlogin;
            }
            else
            {
                return null;
            }


        }

        public User LogOut(string email, string passWord)
        {
            throw new NotImplementedException();
        }


        public User Update(User user)
        {
            var _user = _context.Users.SingleOrDefault(item => (item.Email == user.Email));

            if (_user != null)
            {
                _user.Password = user.Password;
                _user.Name = user.Name;
                _user.DateOfBirth = user.DateOfBirth;
                _user.CreateBy = user.CreateBy;
                _user.UpdateBy = user.UpdateBy;
                _user.CreateTime = DateTime.Now;
                _user.Role_ID = user.Role_ID;
                _context.SaveChanges();
            }


            return _user;
        }

        public User ForgotPassword(User user)
        {
            var _user = _context.Users.SingleOrDefault(item => (item.Email == user.Email));

            if (_user != null)
            {
                string newPassword = GenerateRandomPassword();
                _user.Password = newPassword;
                _context.SaveChanges();
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


        public User Register(User user)
        {
            var existingUser = _context.Users.SingleOrDefault(u => ((u.Email == user.Email) || (u.Phone == user.Phone)));

            if (existingUser != null)
            {
                return null;
            }

            var newUser = new User
            {
                Name = user.Name,
                Password = user.Password,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth.Date,
                CreateBy = user.CreateBy,
                UpdateBy = user.UpdateBy,
                CreateTime = DateTime.Now,
                Role_ID = 1,


            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }
    }
}
