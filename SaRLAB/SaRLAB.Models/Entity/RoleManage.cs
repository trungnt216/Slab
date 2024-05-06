using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("RoleManage")]
    public class RoleManage
    {
        [Key]
        public int RoleID { get; set; }
        public string? RoleName { get; set; }
    }
}
