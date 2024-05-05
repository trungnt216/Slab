using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models
{
    [Table("MutipleChoiceQuestion")]
    public class MultipleChoiceQuestion
    {
        [Key]
        public int Id { get; set; }
        public string? Question { get; set; }
        public String? SelectonA { get; set; }
        public String? SelectonB { get; set; }
        public String? SelectonC { get; set; }
        public String? SelectonD { get; set; }
        public String? Answer { get; set; }
        public int Subject_Id { get; set; }
    }
}
