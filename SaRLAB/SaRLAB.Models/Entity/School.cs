using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("School")]
    public class School
    {
        [Key]
        public int? ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên trường!")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        public string? Address { get; set; }

    }
}
