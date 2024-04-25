using SaRLAB.Models;

namespace SaRLAB.DataAccess.ProjectDto.LoginDto
{
    public interface ILoginDto
    {
        List<User> GetAll();

        User LogIn(string email, string passWord);

        User LogOut(string email, string passWord);

        User Register(string email, string passWord);

        User djjdjdj(string email, string passWord);
    }
}
