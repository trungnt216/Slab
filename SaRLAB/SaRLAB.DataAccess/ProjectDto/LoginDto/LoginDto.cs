using SaRLAB.Models;

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
            };

            return userlogin;
        }
    }
}
