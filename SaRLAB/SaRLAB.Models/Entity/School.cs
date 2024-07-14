using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("School")]
    public class School
    {
        [Key]
        public int? ID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên trường!")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        public string? Address { get; set; }
        public string? ChemLogo { get; set; }
        public string? BioLogo { get; set; }
        public string? PhysLogo { get; set; }
        public string? BiochemLogo { get; set; }
        public string? Banner { get; set; }
        public string? SchoolLogo { get; set; }
        public string? BackupSubject1Logo { get; set; }
        public string? BackupSubject2Logo { get; set; }
        public string? BackupSubject3Logo { get; set; }
        public string? BackupSubject4Logo { get; set; }
        public string? BackupSubject5Logo { get; set; }
        public string? BackupSubject6Logo { get; set; }
        public string? BackupSubject7Logo { get; set; }
        public string? BackupSubject8Logo { get; set; }
        public string? BackupSubject9Logo { get; set; }
        public string? BackupSubject10Logo { get; set; }
        public string? BackupSubject11Logo { get; set; }
        public string? BackupSubject12Logo { get; set; }
        public string? BackupSubject13Logo { get; set; }
        public string? BackupSubject14Logo { get; set; }
        public string? BackupSubject15Logo { get; set; }
        public string? BackupSubject16Logo { get; set; }
        public string? BackupSubject17Logo { get; set; }
        public string? BackupSubject18Logo { get; set; }
        public string? BackupSubject19Logo { get; set; }
        public string? BackupSubject20Logo { get; set; }
        public string? BackupSubject21Logo { get; set; }
        public string? BackupSubject22Logo { get; set; }
        public string? BackupSubject23Logo { get; set; }
        public string? BackupSubject24Logo { get; set; }
        public string? BackupSubject25Logo { get; set; }
        public string? BackupSubject26Logo { get; set; }
        public string? BackupSubject27Logo { get; set; }
        public string? BackupSubject28Logo { get; set; }
        public string? BackupSubject29Logo { get; set; }
        public string? BackupSubject30Logo { get; set; }
        public string? BackupSubject31Logo { get; set; }
        public string? BackupSubject32Logo { get; set; }
    }
}
