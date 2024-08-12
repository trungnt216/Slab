using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaRLAB.Models.Entity
{
    [Table("Subject")]
    public class Subject
    {
        [Key]
        public int ID { get; set; }
        public string? SubjectName { get; set; }
        public string? Rule { get; set; }
        public int? SchoolId { get; set; }
        public int? Type { get; set; }
        public string? VideoBackGround { get; set; }
        public ICollection<PracticePlan>? PracticePlans { get; set; }
        public ICollection<ScientificResearch>? ScientificResearches { get; set; }
    }
}
