using SaRLAB.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Dto.LoginService
{
    public interface ILoginService
    {
        List<LoginDto> GetAll();
        LoginDto Login(string username, string password);
    }
}
