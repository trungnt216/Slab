using SaRLAB.Models.Entity;
using SaRLAB.Models.Dto;

namespace SaRLAB.DataAccess.Service.UserService
{
    public interface IUserService
    {
        List<UserDto> GetAll();

        User GetByID(string email);

        User LogIn(string email, string passWord);

        User LogOut(string email, string passWord);

        User Update(User user);

        User ForgotPassword(User user);

        User Register(User user);
    }
}
