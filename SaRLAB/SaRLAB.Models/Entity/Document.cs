using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("Document")]
    public class Document
    {
        [Key]
        public int? ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        public string Name { get; set; }
        public string? Path { get; set; }
        public Boolean? SpecializedEnglishFlag { get; set; }
        public Boolean? PageFlag { get; set; }
        public string? Type { get; set; }
        public string? Remark { get; set; }
        public int? SchoolId { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}
