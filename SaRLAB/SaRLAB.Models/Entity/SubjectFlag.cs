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
        public string? UserEmail { get; set; }
        public Boolean? MathPermissionFlag { get; set; }
        public Boolean? MathMarkFlag { get; set; }
        public Boolean? PhysicPermissionFlag { get; set; }
        public Boolean? PhysicMarkFlag { get; set; }
        public Boolean? ChemistryPermissionFlag { get; set; }
        public Boolean? ChemistryMarkFlag { get; set; }
        public Boolean? BiologyPermissionFlag { get; set; }
        public Boolean? BiologyMarkFlag { get; set; }
        public Boolean? BackupSubject1MarkFlag { get; set; } = false;
        public Boolean? BackupSubject2MarkFlag { get; set; } = false;
        public Boolean? BackupSubject3MarkFlag { get; set; } = false;
        public Boolean? BackupSubject4MarkFlag { get; set; } = false;
        public Boolean? BackupSubject5MarkFlag { get; set; } = false;
        public Boolean? BackupSubject6MarkFlag { get; set; } = false;
        public Boolean? BackupSubject1PermissionFlag { get; set; } = false;
        public Boolean? BackupSubject2PermissionFlag { get; set; } = false;
        public Boolean? BackupSubject3PermissionFlag { get; set; } = false;
        public Boolean? BackupSubject4PermissionFlag { get; set; } = false;
        public Boolean? BackupSubject5PermissionFlag { get; set; } = false;
        public Boolean? BackupSubject6PermissionFlag { get; set; } = false;

    }
}
