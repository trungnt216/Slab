using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("ScientificResearchFile")]
    public class ScientificResearchFile
    {
        [Key]
        public int ID { get; set; }
        public string? Path { get; set; }
        public string? Type { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int?ScientificResearchId { get; set; }
        public ScientificResearch? ScientificResearch { get; set; }
    }
}
