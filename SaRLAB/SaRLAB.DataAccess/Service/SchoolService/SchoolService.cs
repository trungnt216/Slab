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
                Address = value.Address,
                Branch = value.Branch,
            });
            return school.ToList();
        }

        public School GetSchoolById(int id)
        {
            return _context.Schools.SingleOrDefault(s => s.ID == id);
        }

        public School GetSchoolWithMaxId()
        {
            return _context.Schools.OrderByDescending(s => s.ID).FirstOrDefault(); 
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
                // Reset các logo về null
                school.ChemLogo = null;
                school.PhysLogo = null;
                school.BioLogo = null;
                school.BiochemLogo = null;
                school.Banner = null;
                school.BranchLogo = null;
                school.BackupSubject1Logo = null;
                school.BackupSubject2Logo = null;
                school.BackupSubject3Logo = null;
                school.BackupSubject4Logo = null;
                school.BackupSubject5Logo = null;
                school.BackupSubject6Logo = null;
                school.BackupSubject7Logo = null;
                school.BackupSubject8Logo = null;
                school.BackupSubject9Logo = null;
                school.BackupSubject10Logo = null;
                school.BackupSubject11Logo = null;
                school.BackupSubject12Logo = null;
                school.BackupSubject13Logo = null;
                school.BackupSubject14Logo = null;
                school.BackupSubject15Logo = null;
                school.BackupSubject16Logo = null;
                school.BackupSubject17Logo = null;
                school.BackupSubject18Logo = null;
                school.BackupSubject19Logo = null;
                school.BackupSubject20Logo = null;
                school.BackupSubject21Logo = null;
                school.BackupSubject22Logo = null;
                school.BackupSubject23Logo = null;
                school.BackupSubject24Logo = null;
                school.BackupSubject25Logo = null;
                school.BackupSubject26Logo = null;
                school.BackupSubject27Logo = null;
                school.BackupSubject28Logo = null;
                school.BackupSubject29Logo = null;
                school.BackupSubject30Logo = null;
                school.BackupSubject31Logo = null;
                school.BackupSubject32Logo = null;

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
                school.Name = updatedSchool.Name ?? school.Name;
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
                school.BackupSubject7Logo = updatedSchool.BackupSubject7Logo ?? school.BackupSubject7Logo;
                school.BackupSubject8Logo = updatedSchool.BackupSubject8Logo ?? school.BackupSubject8Logo;
                school.BackupSubject9Logo = updatedSchool.BackupSubject9Logo ?? school.BackupSubject9Logo;
                school.BackupSubject10Logo = updatedSchool.BackupSubject10Logo ?? school.BackupSubject10Logo;
                school.BackupSubject11Logo = updatedSchool.BackupSubject11Logo ?? school.BackupSubject11Logo;
                school.BackupSubject12Logo = updatedSchool.BackupSubject12Logo ?? school.BackupSubject12Logo;
                school.BackupSubject13Logo = updatedSchool.BackupSubject13Logo ?? school.BackupSubject13Logo;
                school.BackupSubject14Logo = updatedSchool.BackupSubject14Logo ?? school.BackupSubject14Logo;
                school.BackupSubject15Logo = updatedSchool.BackupSubject15Logo ?? school.BackupSubject15Logo;
                school.BackupSubject16Logo = updatedSchool.BackupSubject16Logo ?? school.BackupSubject16Logo;
                school.BackupSubject17Logo = updatedSchool.BackupSubject17Logo ?? school.BackupSubject17Logo;
                school.BackupSubject18Logo = updatedSchool.BackupSubject18Logo ?? school.BackupSubject18Logo;
                school.BackupSubject19Logo = updatedSchool.BackupSubject19Logo ?? school.BackupSubject19Logo;
                school.BackupSubject20Logo = updatedSchool.BackupSubject20Logo ?? school.BackupSubject20Logo;
                school.BackupSubject21Logo = updatedSchool.BackupSubject21Logo ?? school.BackupSubject21Logo;
                school.BackupSubject22Logo = updatedSchool.BackupSubject22Logo ?? school.BackupSubject22Logo;
                school.BackupSubject23Logo = updatedSchool.BackupSubject23Logo ?? school.BackupSubject23Logo;
                school.BackupSubject24Logo = updatedSchool.BackupSubject24Logo ?? school.BackupSubject24Logo;
                school.BackupSubject25Logo = updatedSchool.BackupSubject25Logo ?? school.BackupSubject25Logo;
                school.BackupSubject26Logo = updatedSchool.BackupSubject26Logo ?? school.BackupSubject26Logo;
                school.BackupSubject27Logo = updatedSchool.BackupSubject27Logo ?? school.BackupSubject27Logo;
                school.BackupSubject28Logo = updatedSchool.BackupSubject28Logo ?? school.BackupSubject28Logo;
                school.BackupSubject29Logo = updatedSchool.BackupSubject29Logo ?? school.BackupSubject29Logo;
                school.BackupSubject30Logo = updatedSchool.BackupSubject30Logo ?? school.BackupSubject30Logo;
                school.BackupSubject31Logo = updatedSchool.BackupSubject31Logo ?? school.BackupSubject31Logo;
                school.BackupSubject32Logo = updatedSchool.BackupSubject32Logo ?? school.BackupSubject32Logo;
                school.SchoolSumary = updatedSchool.SchoolSumary ?? school.SchoolSumary;
                school.BranchSumary = updatedSchool.BranchSumary ?? school.SchoolSumary;
                school.Branch = updatedSchool.Branch ?? school.SchoolSumary;
                school.BranchLogo = updatedSchool.BranchLogo ?? school.BranchLogo;
                return _context.SaveChanges(); // Returns the number of entities updated
            }
            return 0; // School with given ID not found
        }
    }
}