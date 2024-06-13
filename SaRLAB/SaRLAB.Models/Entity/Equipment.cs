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
        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        public string? Name { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày hết hạn!")]
        public DateTime? ExpiredTime { get; set; }
        public string? Remark { get; set; }
        public int? SubjectId { get; set; }
        public string? Type { get; set;}
        public string? ImagePath { get; set; }
        public int? SchoolId { get; set; }
        public Subject? Subject { get; set; }
        public int? EquipmentQuantity { get; set; }
        public ICollection<PlanDetail>? PlanDetails { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn đặc tính!")]
        public string? Property { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập xuất xử!")]
        public string? From { get; set; }
    }
}
