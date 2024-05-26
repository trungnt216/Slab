using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("User")]
    public class User
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? SubjectId { get; set; }
        public string? AcademyRank { get; set; }
        public int? Experience { get; set; }
        public string? AvtPath { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn 1 trường!")]
        public int? SchoolId { get; set; }
       [ForeignKey("RoleManages")]
        public int Role_ID { get; set; }
        public RoleManage? RoleManages { get; set; }
    }
}
