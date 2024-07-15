using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.WareHouseService
{
    public interface IWareHouseService
    {
        WareHouse GetByID(int id);
        int InsertWareHouse(WareHouse wareHouse);
        int UpdateWareHouseById(int id, WareHouse wareHouse);
        int DeleteWareHouseById(int id);
        List<WareHouse> GetWareHousesByEquipmentId(int equipmentId);
    }
}
