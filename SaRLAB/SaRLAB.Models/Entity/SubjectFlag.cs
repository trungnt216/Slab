using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("SubjectFlag")]
    public  class SubjectFlag
    {
        [Key]
        public int? ID { get; set; }
        public int? UserId { get; set; }
        public Boolean? MathPermissionFlag { get; set; }
        public Boolean? MathMarkFlag { get; set; }
        public Boolean? PhysicPermissionFlag { get; set; }
        public Boolean? PhysicMarkFlag { get; set; }
        public Boolean? ChemistryPermissionFlag { get; set; }
        public Boolean? ChemistryMarkFlag { get; set; }
        public Boolean? BiologyPermissionFlag { get; set; }
        public Boolean? BiologyMarkFlag { get; set; }

    }
}
