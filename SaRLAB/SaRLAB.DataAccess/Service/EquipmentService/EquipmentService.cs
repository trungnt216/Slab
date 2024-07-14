using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.EquipmentService
{
    public class EquipmentService : IEquipmentService
    {
        private readonly ApplicationDbContext _context;

        public EquipmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteEquipmentById(int id)
        {
            var equipmentToDelete = _context.Equipments.Find(id);
            if (equipmentToDelete == null)
                return 0;

            _context.Equipments.Remove(equipmentToDelete);
            return _context.SaveChanges();
        }

        public Equipment GetEquipmentById(int id)
        {
            var equipment = _context.Equipments.SingleOrDefault(item => (item.ID == id));

            if (equipment == null)
            {
                return null;
            }
            else
            {
                return equipment;
            }
        }

        public List<Equipment> GetEquipmentsBySubjectId(int id)
        {
            return _context.Equipments.Where(e => e.SubjectId == id).ToList();
        }

        public List<Equipment> GetEquipmentsByType(int subjectId, string type)
        {
            return _context.Equipments
                .Where(e => e.SubjectId == subjectId && e.Type == type)
                .ToList();
        }

        public List<Equipment> GetEquipmentsByType(int schoolId, int subjectId, string type)
        {
            return _context.Equipments
               .Where(d => d.SchoolId == schoolId && d.SubjectId == subjectId && d.Type == type)
               .ToList();
        }

        public int InsertEquipment(Equipment equipment)
        {
            _context.Equipments.Add(equipment);
            return _context.SaveChanges();
        }

        public int UpdateEquipmentById(int id, Equipment equipment)
        {
            var existingEquipment = _context.Equipments.Find(id);
            if (existingEquipment == null)
                return 0;
            existingEquipment.Name = equipment.Name ?? existingEquipment.Name;
            existingEquipment.Property = equipment.Property ?? existingEquipment.Property;
            existingEquipment.From = equipment.From ?? existingEquipment.From;
            existingEquipment.ImagePath = equipment.ImagePath ?? existingEquipment.ImagePath;
            existingEquipment.UpdateBy = equipment.UpdateBy ?? existingEquipment.UpdateBy;
            existingEquipment.UpdateTime = equipment.UpdateTime ?? existingEquipment.UpdateTime;
            existingEquipment.Remark = equipment.Remark ?? existingEquipment.Remark;
            existingEquipment.EquipmentQuantity = equipment.EquipmentQuantity ?? existingEquipment.EquipmentQuantity;
            existingEquipment.Type = equipment.Type ?? existingEquipment.Type;
            existingEquipment.ExpiredDate = equipment.ExpiredDate ?? existingEquipment.ExpiredDate;
            existingEquipment.ExpiringSoon = equipment.ExpiringSoon ?? existingEquipment.ExpiringSoon;
            existingEquipment.NormalDate = equipment.NormalDate ?? existingEquipment.NormalDate;
            existingEquipment.Unit1 = equipment.Unit1 ?? existingEquipment.Unit1;
            existingEquipment.Unit1Amount = equipment.Unit1Amount ?? existingEquipment.Unit1Amount;
            existingEquipment.Unit2 = equipment.Unit2 ?? existingEquipment.Unit2;
            existingEquipment.Unit2Amount = equipment.Unit2Amount ?? existingEquipment.Unit2Amount;
            return _context.SaveChanges();
        }
    }
}
