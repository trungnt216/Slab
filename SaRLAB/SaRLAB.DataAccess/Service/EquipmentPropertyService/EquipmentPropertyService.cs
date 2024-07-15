using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.EquipmentPropertyService
{
    public class EquipmentPropertyService : IEquipmentPropertyService
    {

        private readonly ApplicationDbContext _context;

        public EquipmentPropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int DeleteEquipmentPropertyById(int id)
        {
            var EquipmentProper = _context.EquipmentProperties.Find(id);
            if (EquipmentProper != null)
            {
                _context.EquipmentProperties.Remove(EquipmentProper);
                return _context.SaveChanges();
            }
            return 0;
        }

        public List<EquipmentProperty> GetEquipmentPropertiesByEquipmentId(int equipmentId)
        {
            return _context.EquipmentProperties.Where(q => q.EquipmentId == equipmentId).ToList();
        }

        public EquipmentProperty GetEquipmentPropertyById(int id)
        {
            return _context.EquipmentProperties.Find(id);
        }

        public int InsertEquipmentProperty(EquipmentProperty equipmentProperty)
        {
            _context.EquipmentProperties.Add(equipmentProperty);
            return _context.SaveChanges();
        }

        public int UpdateEquipmentPropertyById(int id, EquipmentProperty equipmentProperty)
        {
            var _equipmentProperty = _context.EquipmentProperties.SingleOrDefault(item => (item.ID == id));

            if (_equipmentProperty != null)
            {
                _equipmentProperty.Image = equipmentProperty.Image ?? _equipmentProperty.Image;
                _equipmentProperty.Remark = equipmentProperty.Remark ?? _equipmentProperty.Remark;
                return _context.SaveChanges();
            }
            return 0;
        }
    }
}
