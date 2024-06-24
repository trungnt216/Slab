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

        public int RecoverSchool(int id)
        {
            var school = _context.Schools.FirstOrDefault(s => s.ID == id);
            if (school != null)
            {
                // Update properties of the existing school with the properties of the updated school
                school.ChemLogo = null;
                school.PhysLogo = null;
                school.BioLogo = null;
                school.BiochemLogo = null;
                school.Banner = null;
                school.BackupSubject1Logo = null;
                school.BackupSubject2Logo = null;
                school.BackupSubject3Logo = null;
                school.BackupSubject4Logo = null;
                school.BackupSubject5Logo = null;
                school.BackupSubject6Logo = null;

                return _context.SaveChanges(); // Returns the number of entities updated
            }
            return 0; // School with given ID not found
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
                school.BackupSubject1Logo = updatedSchool.BackupSubject1Logo ?? school.BackupSubject1Logo;
                school.BackupSubject2Logo = updatedSchool.BackupSubject2Logo ?? school.BackupSubject2Logo;
                school.BackupSubject3Logo = updatedSchool.BackupSubject3Logo ?? school.BackupSubject3Logo;
                school.BackupSubject4Logo = updatedSchool.BackupSubject4Logo ?? school.BackupSubject4Logo;
                school.BackupSubject5Logo = updatedSchool.BackupSubject5Logo ?? school.BackupSubject5Logo;
                school.BackupSubject6Logo = updatedSchool.BackupSubject6Logo ?? school.BackupSubject6Logo;
                return _context.SaveChanges(); // Returns the number of entities updated
            }
            return 0; // School with given ID not found
        }
    }
}