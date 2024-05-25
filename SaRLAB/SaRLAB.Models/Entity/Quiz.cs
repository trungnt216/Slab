using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    public class Quiz
    {
        [Key]
        public int? ID { get; set; }
        public string Question { get; set; }
        public string? QuestionImage { get; set; }
        public string OptionA { get; set; }
        public string? OptionAImage { get; set; }
        public string? OptionB { get; set; }
        public string? OptionBImage { get; set; }
        public string? OptionC { get; set; }
        public string? OptionCImage { get; set; }
        public string? OptionD { get; set; }
        public string? OptionDImage { get; set; }
        public char? CorrectAnswer { get; set; }
        public int? SchoolId { get; set; }
        public int? SubjectId { get; set; }
    }

}
