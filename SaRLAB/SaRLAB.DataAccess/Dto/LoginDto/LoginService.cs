using Microsoft.EntityFrameworkCore;
using SaRLAB.Models.Dto;
using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Dto.LoginService
{
    public class LoginService : ILoginService
    {

        private readonly ApplicationDbContext _context;

        public LoginService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<LoginDto> GetAll()
        {
            var _Login = _context.Users.Select(value => new LoginDto
            {
               Email = value.Email,
               ID = value.ID,
               Name = value.Name,
               Password = value.Password,
               RoleName = value.RoleManages.RoleName,

            });

            return _Login.ToList();
        }

        public  LoginDto Login(string username, string password)
        {
             var user = _context.Users
                 .Include(u => u.RoleManages) // Đảm bảo RoleManages (vai trò) được load lên
                 .FirstOrDefault(u => u.Email == username && u.Password == password);

            if (user != null)
            {
                LoginDto userlogin = new LoginDto
                {
                    Email = user.Email,
                    ID = user.ID,
                    Name = user.Name,
                    RoleName = user.RoleManages.RoleName
                };

                return userlogin;
            }
            else
            {
                return null;
            }
        }


    }
}
