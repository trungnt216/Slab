using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Dto
{
    public class TopicScientificResearchDto
    {
        public int ID { get; set; }

        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? PublicationDate { get; set; }
    }
}
