using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.BannerService
{
    public interface IBannerService
    {
        List<Banner> GetAll();
        Banner Insert(Banner banner);
        Banner Update(Banner banner);
        Banner GetById(int id);
        void DeleteById(int id);
    }
}
