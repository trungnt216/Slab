using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("Equipment")]
    public class Equipment
    {
        [Key]
        public int? ID { get; set; }
        public string? Name { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? DueDateTime { get; set; }
        public bool? Status { get; set; }
        public string? type { get; set; }
        public string? description { get; set; }
        public string? pathImage { get; set; }
        public int? SubjectId { get; set; }
        public int? SchoolId { get; set; }
        public Subject? Subject { get; set; }
        public string? Remark { get; set; }
        public int? EquipmentQuantity { get; set; }
        public ICollection<PlanDetail>? PlanDetails { get; set; }
    }
}
