using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.EquipmentService
{
    public interface IEquipmentService
    {
        List<Equipment> GetEquipmentsBySubjectId(int id);
        Equipment GetEquipmentById(int id);
        int InsertEquipment(Equipment equipment);
        int UpdateEquipmentById(int id, Equipment equipment);
        int DeleteEquipmentById(int id);
    }
}
