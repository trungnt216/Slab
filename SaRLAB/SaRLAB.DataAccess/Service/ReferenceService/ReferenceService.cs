using Microsoft.EntityFrameworkCore;
using SaRLAB.Models.Entity;
using System.Collections.Generic;
using System.Linq;

namespace SaRLAB.DataAccess.Service.ReferenceService
{
    public class ReferenceService : IReferenceService
    {
        private readonly ApplicationDbContext _context;

        public ReferenceService(ApplicationDbContext context)
        {
            _context = context;
        }
        public int DeleteReferenceById(int id)
        {
            var referenceDelete = _context.References.Find(id);
            if (referenceDelete == null)
                return 0;

            _context.References.Remove(referenceDelete);
            return _context.SaveChanges();
        }

        public Reference GetReferenceById(int id)
        {
            var reference = _context.References.SingleOrDefault(item => (item.ID == id));

            if (reference == null)
            {
                return null;
            }
            else
            {
                return reference;
            }
        }

        public List<Reference> GetReferenceByName(string name)
        {
            var lowerCaseName = name.ToLower();
            return _context.References
                   .Where(r => r.Name.ToLower().Contains(lowerCaseName))
                   .ToList();
        }

        public List<Reference> GetReferenceByType(string type, int schoolID)
        {
            var lowerCaseName = type.ToLower();

            return _context.References
                   .Where(r => r.Type.ToLower().Contains(lowerCaseName) && r.SchoolId == schoolID)
                   .ToList();
        }


        public List<Reference> GetReferencesBySchoolId(int schoolId)
        {
            return _context.References
                   .Where(d => d.SchoolId == schoolId)
                  .ToList();
        }

        public int InsertReference(Reference reference)
        {
            _context.References.Add(reference);
            return _context.SaveChanges();
        }

        public int UpdateReferenceById(int id, Reference reference)
        {
            var existingReference = _context.References.FirstOrDefault(d => d.ID == id);

            if (existingReference == null)
            {
                return 0; 
            }
            existingReference.Name = reference.Name ?? existingReference.Name;
            existingReference.Path = reference.Path ?? existingReference.Path;
            existingReference.Remark = reference.Remark ?? existingReference.Remark;
            existingReference.Author = reference.Author ?? existingReference.Author;
            existingReference.Summary = reference.Summary ?? existingReference.Summary;
            existingReference.Path = reference.Path ?? existingReference.Path;
            existingReference.PublicationYear = reference.PublicationYear ?? existingReference.PublicationYear;
            existingReference.Publisher = reference.Publisher ?? existingReference.Publisher;
            existingReference.SchoolId = existingReference.SchoolId;
            _context.SaveChanges();

            return 1; 
        }
    }
}
