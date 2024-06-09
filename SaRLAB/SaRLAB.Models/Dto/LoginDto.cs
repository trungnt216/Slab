using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.Models.Dto
{
    public class LoginDto
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? RoleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? SchoolId { get; set;}
        public string? AvtPath { get; set; }
    }
}
