using SaRLAB.Models;

namespace SaRLAB.DataAccess.ProjectDto.LoginDto
{
    public interface ILoginDto
    {
        List<User> GetAll();

        User LogIn(string email, string passWord);
    }
}
