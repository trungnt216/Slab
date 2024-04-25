

using SaRLAB.MobileApp.Models;

namespace SaRLAB.MobileApp.Dto;

public interface IUserDto
{
    Task<User> Login(string email, string password);
}
