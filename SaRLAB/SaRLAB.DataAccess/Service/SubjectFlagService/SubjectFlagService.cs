using SaRLAB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.Service.SubjectFlagService
{
    public class SubjectFlagService : ISubjectFlagService
    {
        private readonly ApplicationDbContext _context;

        public SubjectFlagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int DeleteByUserEmail(string userEmail)
        {
            var flagsToDelete = _context.SubjectFlags.Where(u => u.UserEmail == userEmail).ToList();

            if (flagsToDelete.Any())
            {
                _context.SubjectFlags.RemoveRange(flagsToDelete);
                _context.SaveChanges();
                return flagsToDelete.Count;
            }

            return 0;
        }

        public void DeleteByUserEmails(string userEmails)
        {
            var emails = userEmails.Split(',');
            var flagsToDelete = _context.SubjectFlags.Where(u => emails.Contains(u.UserEmail)).ToList();

            if (flagsToDelete.Any())
            {
                _context.SubjectFlags.RemoveRange(flagsToDelete);
                _context.SaveChanges();
            }
        }

        public SubjectFlag getSubjectFlagByUserEmail(String userEmail)
        {
            var subjectFlag = _context.SubjectFlags.SingleOrDefault(item => item.UserEmail == userEmail);

            if (subjectFlag != null)
            {
                SubjectFlag _subjectFlag = new SubjectFlag
                {
                    ID = subjectFlag.ID,
                    UserEmail = subjectFlag.UserEmail,
                    MathPermissionFlag = subjectFlag.MathPermissionFlag,
                    MathMarkFlag = subjectFlag.MathMarkFlag,
                    PhysicPermissionFlag = subjectFlag.PhysicPermissionFlag,
                    PhysicMarkFlag = subjectFlag.PhysicMarkFlag,
                    BiologyPermissionFlag = subjectFlag.BiologyPermissionFlag,
                    BiologyMarkFlag = subjectFlag.BiologyMarkFlag,
                    ChemistryPermissionFlag = subjectFlag.ChemistryPermissionFlag,
                    ChemistryMarkFlag = subjectFlag.ChemistryMarkFlag,

                };

                return _subjectFlag;
            }
            else
            {
                return null;
            }
        }

        public int InsertSubjectFlag(String userEmail)
        {
            var checkSubjectFlag = _context.SubjectFlags.SingleOrDefault(item => (item.UserEmail == userEmail));

            if (checkSubjectFlag != null)
            {
                return 0;
            }

            var newSubjectFlag = new SubjectFlag
            {
                UserEmail = userEmail,
                MathPermissionFlag = false,
                MathMarkFlag = false,
                PhysicPermissionFlag = false,
                PhysicMarkFlag = false,
                BiologyPermissionFlag = false,
                BiologyMarkFlag = false,
                ChemistryPermissionFlag = false,
                ChemistryMarkFlag = false,
            };

            _context.SubjectFlags.Add(newSubjectFlag);
            _context.SaveChanges();

            return 1;
        }

        public int updateSubjectFlag(String userEmail, SubjectFlag subjectFlag)
        {
            var _subjectFlag = _context.SubjectFlags.SingleOrDefault(item => (item.UserEmail == subjectFlag.UserEmail));

            if (_subjectFlag != null)
            {
                _subjectFlag.UserEmail = subjectFlag.UserEmail;
                _subjectFlag.MathPermissionFlag = subjectFlag.MathPermissionFlag ?? _subjectFlag.MathPermissionFlag;
                _subjectFlag.MathMarkFlag = subjectFlag.MathMarkFlag ?? _subjectFlag.MathMarkFlag;
                _subjectFlag.PhysicPermissionFlag = subjectFlag.PhysicPermissionFlag ?? _subjectFlag.PhysicPermissionFlag;
                _subjectFlag.PhysicMarkFlag = subjectFlag.PhysicMarkFlag ?? _subjectFlag.PhysicMarkFlag;
                _subjectFlag.BiologyPermissionFlag = subjectFlag.BiologyPermissionFlag ?? _subjectFlag.BiologyPermissionFlag;
                _subjectFlag.BiologyMarkFlag = subjectFlag.BiologyMarkFlag ?? _subjectFlag.BiologyMarkFlag;
                _subjectFlag.BiologyPermissionFlag = subjectFlag.BiologyPermissionFlag ?? _subjectFlag.BiologyPermissionFlag;
                _subjectFlag.ChemistryPermissionFlag = subjectFlag.ChemistryPermissionFlag ?? _subjectFlag.ChemistryPermissionFlag;
                _subjectFlag.ChemistryMarkFlag = subjectFlag.ChemistryMarkFlag ?? _subjectFlag.ChemistryMarkFlag;
                _context.SaveChanges();
            }
            return 1;
        }
    }
}
