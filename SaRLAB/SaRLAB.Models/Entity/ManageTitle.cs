using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaRLAB.Models.Entity
{
    [Table("ManageTitle")]
    public class ManageTitle
    {
        [Key]
        public int ID { get; set; }
        public int? SchoolId { get; set; } // ID của trường học
        public int? SubjectId { get; set; } // ID của môn học
        public string? Title { get; set; } // Tiêu đề
        public int? Type { get; set; }
    }
}