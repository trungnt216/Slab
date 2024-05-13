using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    public class Equipment
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public string? Remark { get; set; }
        public int? EquipmentQuantity { get; set; }
        public ICollection<PlanDetail>? PlanDetails { get; set; }

    }
}
