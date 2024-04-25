using SaRLAB.Models;

namespace SaRLAB.DataAccess.ProjectDto.LoginDto
{
    public interface ILoginDto
    {
        List<User> GetAll();

        User LogIn(string email, string passWord);

        User LogOut();

        User Update(User user);

        User ForgotPassword(User user);
    }
}
