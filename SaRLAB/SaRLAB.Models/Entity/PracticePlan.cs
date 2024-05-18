using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("PracticePlan")]
    public class PracticePlan
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? PracticeType { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? SubjectId  { get; set; }
        public Subject? Subject { get; set; }

        public ICollection<PlanDetail>?  PlanDetails { get; set; }

    }
}
