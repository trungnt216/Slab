using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Entity
{
    [Table("ManageLogic")]
    public class ManageLogic
    {
        [Key]
        public int id { get; set; }
        public string? email { get; set; }
    }
}
