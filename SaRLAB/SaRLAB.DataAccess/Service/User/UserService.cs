using System.Net;
using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;
using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;

namespace SaRLAB.DataAccess.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<UserDto> GetAll()
        {
            var user = _context.Users.Select(value => new UserDto
            {
                ID = value.ID,
                Phone = value.Phone,
                Email = value.Email,
                Name = value.Name,
                CreateBy = value.CreateBy,
                UpdateBy = value.UpdateBy,
                CreateTime = value.CreateTime,
                RoleName = value.RoleManages.RoleName,
                AvtPath = value.AvtPath,
                DateOfBirth = value.DateOfBirth,
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


        public User GetByID(string email)
        {
            var user = _context.Users.SingleOrDefault(item => (item.Email == email));

            if (user == null)
            {
                return null;
            }
            else
            {
                return user;
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
                _user.AvtPath = user.AvtPath;
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
                Role_ID = 5,
                AvtPath = user.AvtPath,
                SchoolId = user.SchoolId
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

        public void DeleteById(int userId)
        {
            var userToDelete = _context.Users.Find(userId);
            if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                _context.SaveChanges();
            }
        }

        public void DeleteByIds(string userIds)
        {
            // Split the string into individual IDs
            var ids = userIds.Split(',');

            // Convert string IDs to integers
            var userIdIntegers = ids.Select(id => int.Parse(id)).ToList();

            // Find and delete users with the specified IDs
            var usersToDelete = _context.Users.Where(u => userIdIntegers.Contains(u.ID)).ToList();

            if (usersToDelete.Any())
            {
                _context.Users.RemoveRange(usersToDelete);
                _context.SaveChanges();
            }
        }

        public List<User> GetAllUser()
        {
            var user = _context.Users.Select(value => new User
            {
                ID = value.ID,
                Phone = value.Phone,
                Email = value.Email,
                Name = value.Name,
                CreateBy = value.CreateBy,
                UpdateBy = value.UpdateBy,
                CreateTime = value.CreateTime,
                AvtPath = value.AvtPath,
                DateOfBirth = value.DateOfBirth,
                RoleManages = value.RoleManages,
                Role_ID = value.Role_ID,
            });
            return user.ToList();
        }

        public List<User> SearchUsers(string? name, string? email, int? roleId)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.Role_ID == roleId.Value);
            }

            return query.ToList();
        }

        public List<User> GetUsersByRole(int role, int schoolId, int? subjectId)
        {
            IQueryable<User> query = _context.Users.Where(u => u.Role_ID == role && u.SchoolId == schoolId);

            if (subjectId.HasValue && subjectId != 0)
            {
                query = query.Where(u => u.SubjectId == subjectId);
            }

            return query.ToList();
        }
    }
}
