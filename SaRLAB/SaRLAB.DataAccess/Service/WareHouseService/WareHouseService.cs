using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.WareHouseService
{
    public class WareHouseService : IWareHouseService
    {
        private readonly ApplicationDbContext _context;

        public WareHouseService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteWareHouseById(int id)
        {
            var wareHouse = _context.WareHouses.Find(id);
            if (wareHouse != null)
            {
                _context.WareHouses.Remove(wareHouse);
                return _context.SaveChanges();
            }
            return 0;
        }

        public WareHouse GetByID(int id)
        {
            return _context.WareHouses.SingleOrDefault(s => s.ID == id);
        }

        public List<WareHouse> GetWareHousesByEquipmentId(int equipmentId)
        {
            return _context.WareHouses.Where(q => q.EquipmentId == equipmentId).ToList();
        }

        public int InsertWareHouse(WareHouse wareHouse)
        {
            _context.WareHouses.Add(wareHouse);
            return _context.SaveChanges();
        }

        public int UpdateWareHouseById(int id, WareHouse wareHouse)
        {
            var _wareHouse = _context.WareHouses.SingleOrDefault(item => (item.ID == id));

            if (_wareHouse != null)
            {
                _wareHouse.Name = wareHouse.Name ?? _wareHouse.Name;
                _wareHouse.AmountEquipment = wareHouse.AmountEquipment ?? _wareHouse.AmountEquipment;
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
