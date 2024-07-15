using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaRLAB.Models.Entity
{
    [Table("EquipmentProperty")]
    public class EquipmentProperty
    {
        [Key]
        public int ID { get; set; }  // Khóa chính
        public int EquipmentId { get; set; }  // Khóa ngoại tham chiếu đến Equipment
        public string? Image { get; set; }  // Đường dẫn đến hình ảnh
        public string? Remark { get; set; }  // Ghi chú về thiết bị

    }
}
