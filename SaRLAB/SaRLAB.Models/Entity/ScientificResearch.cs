﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{

    [Table("ScientificResearch")]
    public class ScientificResearch
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Author { get; set; }
        public string? CreateBy { get; set; }
        public string? remark { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string UserVerifyFlag { get; set; }
        public string AdminVerifyFlag { get; set; }
        public ICollection<ScientificResearchFile>? ScientificResearchFiles { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}

