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
        List<Equipment> GetEquipmentsByType(int subjectId,string type);
        Equipment GetEquipmentById(int id);
        int InsertEquipment(Equipment equipment);
        int UpdateEquipmentById(int id, Equipment equipment);
        int DeleteEquipmentById(int id);
        List<Equipment> GetEquipmentsByType(int schoolId, int subjectId, string type);
    }
}
