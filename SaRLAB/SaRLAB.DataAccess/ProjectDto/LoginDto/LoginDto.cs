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

        public User Register(User user)
        {
            var existingUser = _context.User.SingleOrDefault(u => u.Email == user.Email);

            if (existingUser != null)
            {
                return null;
            }

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Phone = user.Phone,
                Role = "Student",
                DateOfBirth = user.DateOfBirth.Date


            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }

        public User ádsa(string name, string email, string passWord, string phone, DateTime dateOfBirth)
        {
            var existingUser = _context.User.SingleOrDefault(u => u.Email == email);

            if (existingUser != null)
            {
                return null; 
            }

            var newUser = new User
            {
                Name = name,
                Email = email,
                Password = passWord,
                Phone = phone,
                Role = "Student",
                DateOfBirth = dateOfBirth

            };

            _context.User.Add(newUser);
            _context.SaveChanges();

            return newUser;
        }
    }
}
