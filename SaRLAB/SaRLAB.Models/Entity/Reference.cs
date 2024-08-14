using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SaRLAB.Models.Entity
{
    [Table("Reference")]
    public class Reference
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public int? PublicationYear { get; set; }
        public string? Publisher { get; set; }
        public string? Summary { get; set; }
        public string? Remark { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? CoverImage { get; set; }
        public string? Path { get; set; }
        public int? SchoolId { get; set; }
    }
}
