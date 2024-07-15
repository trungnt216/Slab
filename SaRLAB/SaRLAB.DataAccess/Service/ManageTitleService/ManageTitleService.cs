using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.ManageTitleService
{
    public class ManageTitleService : IManageTitleService
    {

        private readonly ApplicationDbContext _context;

        public ManageTitleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int DeleteManageTitleById(int id)
        {
            var manageTitle = _context.ManageTitles.Find(id);
            if (manageTitle != null)
            {
                _context.ManageTitles.Remove(manageTitle);
                return _context.SaveChanges();
            }
            return 0;
        }

        public ManageTitle GetManageTitleAccordingSchoolAndSubjectAndType(int schoolId, int subjectId, int type)
        {
            var manageTitle = _context.ManageTitles
                .SingleOrDefault(item => item.SchoolId == schoolId && item.SubjectId == subjectId && item.Type == type);
            return manageTitle != null ? new ManageTitle
            {
                ID = manageTitle.ID,
                SubjectId = manageTitle.SubjectId,
                SchoolId = manageTitle.SchoolId,
                Title = manageTitle.Title,
                Type = manageTitle.Type
            } : null;
        }

        public List<ManageTitle> GetManageTitlesAccordingSchool(int schoolId)
        {
            return _context.ManageTitles.Where(q => q.SchoolId == schoolId).ToList();
        }

        public List<ManageTitle> GetManageTitlesAccordingSchoolAndSubject(int schoolId, int subjectId)
        {
            return _context.ManageTitles.Where(q => q.SchoolId == schoolId && q.SubjectId == subjectId).ToList();
        }

        public int InsertManageTitle(ManageTitle manageTitle)
        {
            _context.ManageTitles.Add(manageTitle);
            return _context.SaveChanges();
        }

        public int UpdateManageTitleById(int id, ManageTitle manageTitle)
        {
            var _manageTitle = _context.ManageTitles.SingleOrDefault(item => (item.ID == id));

            if (_manageTitle != null)
            {
                _manageTitle.Title = manageTitle.Title ?? _manageTitle.Title;
                _manageTitle.Type = manageTitle.Type ?? _manageTitle.Type;
                return _context.SaveChanges();
            }
            return 0;
        }
    }
  }

