using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("PlanDetail")]
    public class  PlanDetail
    {
        [Key]
        public int? ID { get; set; }

        public int? EquipmentQuantity { get; set; }
        public int? EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
        public int? PracticePlanId { get; set; }
        public PracticePlan? PracticePlan { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
