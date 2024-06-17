using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.SchoolService
{
    public class SchoolService : ISchoolService
    {
        private readonly ApplicationDbContext _context;

        public SchoolService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteSchoolById(int id)
        {
            var school = _context.Schools.FirstOrDefault(s => s.ID == id);
            if (school != null)
            {
                _context.Schools.Remove(school);
                return _context.SaveChanges(); // Returns the number of entities deleted
            }
            return 0; // School with given ID not found
        }

        public List<School> GetAllSchool()
        {
            var school = _context.Schools.Select(value => new School
            {
                ID = value.ID.Value,
                Name = value.Name,
                Address = value.Address
            });
            return school.ToList();
        }

        public School GetSchoolById(int id)
        {
            return _context.Schools.SingleOrDefault(s => s.ID == id);
        }

        public int InsertSchool(School school)
        {
            _context.Schools.Add(school);
            return _context.SaveChanges();
        }

        public int UpdateSchoolById(int id, School updatedSchool)
        {
            var school = _context.Schools.FirstOrDefault(s => s.ID == id);
            if (school != null)
            {
                // Update properties of the existing school with the properties of the updated school
                school.Name =  updatedSchool.Name ?? school.Name;
                school.Address = updatedSchool.Address ?? school.Address;
                school.ChemLogo = updatedSchool.ChemLogo ?? school.ChemLogo;
                school.PhysLogo = updatedSchool.PhysLogo ?? school.PhysLogo;
                school.BioLogo = updatedSchool.BioLogo ?? school.BioLogo;
                school.BiochemLogo = updatedSchool.BiochemLogo ?? school.BiochemLogo;
                school.Banner = updatedSchool.Banner ?? school.Banner;
                school.SchoolLogo = updatedSchool.SchoolLogo ?? school.SchoolLogo;
                return _context.SaveChanges(); // Returns the number of entities updated
            }
            return 0; // School with given ID not found
        }
    }
}