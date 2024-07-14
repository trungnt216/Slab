using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaRLAB.Models.Entity
{
    [Table("SubjectFlag")]
    public class SubjectFlag
    {
        [Key]
        public int? ID { get; set; }
        public string? UserEmail { get; set; }
        public bool? MathPermissionFlag { get; set; }
        public bool? MathMarkFlag { get; set; }
        public bool? PhysicPermissionFlag { get; set; }
        public bool? PhysicMarkFlag { get; set; }
        public bool? ChemistryPermissionFlag { get; set; }
        public bool? ChemistryMarkFlag { get; set; }
        public bool? BiologyPermissionFlag { get; set; }
        public bool? BiologyMarkFlag { get; set; }

        public bool? BackupSubject1MarkFlag { get; set; } = false;
        public bool? BackupSubject2MarkFlag { get; set; } = false;
        public bool? BackupSubject3MarkFlag { get; set; } = false;
        public bool? BackupSubject4MarkFlag { get; set; } = false;
        public bool? BackupSubject5MarkFlag { get; set; } = false;
        public bool? BackupSubject6MarkFlag { get; set; } = false;

        public bool? BackupSubject1PermissionFlag { get; set; } = false;
        public bool? BackupSubject2PermissionFlag { get; set; } = false;
        public bool? BackupSubject3PermissionFlag { get; set; } = false;
        public bool? BackupSubject4PermissionFlag { get; set; } = false;
        public bool? BackupSubject5PermissionFlag { get; set; } = false;
        public bool? BackupSubject6PermissionFlag { get; set; } = false;

        // Bổ sung từ BackupSubject7 đến BackupSubject32
        public bool? BackupSubject7MarkFlag { get; set; } = false;
        public bool? BackupSubject7PermissionFlag { get; set; } = false;
        public bool? BackupSubject8MarkFlag { get; set; } = false;
        public bool? BackupSubject8PermissionFlag { get; set; } = false;
        public bool? BackupSubject9MarkFlag { get; set; } = false;
        public bool? BackupSubject9PermissionFlag { get; set; } = false;
        public bool? BackupSubject10MarkFlag { get; set; } = false;
        public bool? BackupSubject10PermissionFlag { get; set; } = false;
        public bool? BackupSubject11MarkFlag { get; set; } = false;
        public bool? BackupSubject11PermissionFlag { get; set; } = false;
        public bool? BackupSubject12MarkFlag { get; set; } = false;
        public bool? BackupSubject12PermissionFlag { get; set; } = false;
        public bool? BackupSubject13MarkFlag { get; set; } = false;
        public bool? BackupSubject13PermissionFlag { get; set; } = false;
        public bool? BackupSubject14MarkFlag { get; set; } = false;
        public bool? BackupSubject14PermissionFlag { get; set; } = false;
        public bool? BackupSubject15MarkFlag { get; set; } = false;
        public bool? BackupSubject15PermissionFlag { get; set; } = false;
        public bool? BackupSubject16MarkFlag { get; set; } = false;
        public bool? BackupSubject16PermissionFlag { get; set; } = false;
        public bool? BackupSubject17MarkFlag { get; set; } = false;
        public bool? BackupSubject17PermissionFlag { get; set; } = false;
        public bool? BackupSubject18MarkFlag { get; set; } = false;
        public bool? BackupSubject18PermissionFlag { get; set; } = false;
        public bool? BackupSubject19MarkFlag { get; set; } = false;
        public bool? BackupSubject19PermissionFlag { get; set; } = false;
        public bool? BackupSubject20MarkFlag { get; set; } = false;
        public bool? BackupSubject20PermissionFlag { get; set; } = false;
        public bool? BackupSubject21MarkFlag { get; set; } = false;
        public bool? BackupSubject21PermissionFlag { get; set; } = false;
        public bool? BackupSubject22MarkFlag { get; set; } = false;
        public bool? BackupSubject22PermissionFlag { get; set; } = false;
        public bool? BackupSubject23MarkFlag { get; set; } = false;
        public bool? BackupSubject23PermissionFlag { get; set; } = false;
        public bool? BackupSubject24MarkFlag { get; set; } = false;
        public bool? BackupSubject24PermissionFlag { get; set; } = false;
        public bool? BackupSubject25MarkFlag { get; set; } = false;
        public bool? BackupSubject25PermissionFlag { get; set; } = false;
        public bool? BackupSubject26MarkFlag { get; set; } = false;
        public bool? BackupSubject26PermissionFlag { get; set; } = false;
        public bool? BackupSubject27MarkFlag { get; set; } = false;
        public bool? BackupSubject27PermissionFlag { get; set; } = false;
        public bool? BackupSubject28MarkFlag { get; set; } = false;
        public bool? BackupSubject28PermissionFlag { get; set; } = false;
        public bool? BackupSubject29MarkFlag { get; set; } = false;
        public bool? BackupSubject29PermissionFlag { get; set; } = false;
        public bool? BackupSubject30MarkFlag { get; set; } = false;
        public bool? BackupSubject30PermissionFlag { get; set; } = false;
        public bool? BackupSubject31MarkFlag { get; set; } = false;
        public bool? BackupSubject31PermissionFlag { get; set; } = false;
        public bool? BackupSubject32MarkFlag { get; set; } = false;
        public bool? BackupSubject32PermissionFlag { get; set; } = false;
    }
}
