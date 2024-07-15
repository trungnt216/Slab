using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.EquipmentPropertyService
{
    public interface IEquipmentPropertyService
    {
        int InsertEquipmentProperty(EquipmentProperty equipmentProperty);
        int UpdateEquipmentPropertyById(int id, EquipmentProperty equipmentProperty);
        int DeleteEquipmentPropertyById(int id);
        EquipmentProperty GetEquipmentPropertyById(int id);
        List<EquipmentProperty> GetEquipmentPropertiesByEquipmentId(int equipmentId);
    }
}
