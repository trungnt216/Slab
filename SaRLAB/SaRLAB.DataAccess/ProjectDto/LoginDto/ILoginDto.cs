using SaRLAB.Models;

namespace SaRLAB.DataAccess.ProjectDto.LoginDto
{
    public interface ILoginDto
    {
        List<User> GetAll();

        User LogIn(string email, string passWord);

        User LogOut(string email, string passWord);

        User Update(User user);

        User ForgotPassword(User user);

        User Register(User user);
    }
}
