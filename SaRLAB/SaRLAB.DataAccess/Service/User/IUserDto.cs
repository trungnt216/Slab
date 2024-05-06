using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.UserDto
{
    public interface IUserDto
    {
        List<User> GetAll();

        User LogIn(string email, string passWord);

        User LogOut(string email, string passWord);

        User Update(User user);

        User ForgotPassword(User user);

        User Register(User user);
    }
}
