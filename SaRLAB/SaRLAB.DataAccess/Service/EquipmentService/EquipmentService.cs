﻿using SaRLAB.Models.Entity;
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

            existingEquipment.Name = equipment.Name;
            existingEquipment.CreateBy = equipment.CreateBy;
            existingEquipment.CreateTime = equipment.CreateTime;
            existingEquipment.UpdateBy = equipment.UpdateBy;
            existingEquipment.UpdateTime = equipment.UpdateTime;
            existingEquipment.SubjectId = equipment.SubjectId;
            existingEquipment.Remark = equipment.Remark;
            existingEquipment.EquipmentQuantity = equipment.EquipmentQuantity;

            return _context.SaveChanges();
        }
    }
}