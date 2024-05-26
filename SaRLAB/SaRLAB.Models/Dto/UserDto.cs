using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Dto
{
    public class UserDto
    {
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
        public string? RoleName { get; set; }
        public string? AvtPath { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn trường!")]
        public int? SchoolId { get; set; }
    }
}
