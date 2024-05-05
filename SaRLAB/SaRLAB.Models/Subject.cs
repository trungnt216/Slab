using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models
{
    [Table("Subject")]
    public class Subject
    {
        [Key]
        public int ID { get; set; }
        public string? SubjectName { get; set; }
        public string? Manage { get; set; }
        public string? Rule { get; set; }
    }
}
