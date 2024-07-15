using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("WareHouse")]
    public class WareHouse
    {
        [Key]
        public int? ID { get; set; }
        public int? EquipmentId { get; set; }
        public string? Name { get; set; }
        public Double? AmountEquipment { get; set; }

    }
}
