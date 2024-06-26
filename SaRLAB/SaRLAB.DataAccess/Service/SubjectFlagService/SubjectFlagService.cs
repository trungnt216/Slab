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
                    BackupSubject1MarkFlag = subjectFlag.BackupSubject1MarkFlag,
                    BackupSubject2MarkFlag = subjectFlag.BackupSubject2MarkFlag,
                    BackupSubject3MarkFlag = subjectFlag.BackupSubject3MarkFlag,
                    BackupSubject4MarkFlag = subjectFlag.BackupSubject4MarkFlag,
                    BackupSubject5MarkFlag = subjectFlag.BackupSubject5MarkFlag,
                    BackupSubject6MarkFlag = subjectFlag.BackupSubject6MarkFlag,
                    BackupSubject1PermissionFlag = subjectFlag.BackupSubject1PermissionFlag,
                    BackupSubject2PermissionFlag = subjectFlag.BackupSubject2PermissionFlag,
                    BackupSubject3PermissionFlag = subjectFlag.BackupSubject3PermissionFlag,
                    BackupSubject4PermissionFlag = subjectFlag.BackupSubject4PermissionFlag,
                    BackupSubject5PermissionFlag = subjectFlag.BackupSubject5PermissionFlag,
                    BackupSubject6PermissionFlag = subjectFlag.BackupSubject6PermissionFlag,

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
                BackupSubject1MarkFlag = false,
                BackupSubject2MarkFlag = false,
                BackupSubject3MarkFlag = false,
                BackupSubject4MarkFlag = false,
                BackupSubject5MarkFlag = false,
                BackupSubject6MarkFlag = false,
                BackupSubject1PermissionFlag = false,
                BackupSubject2PermissionFlag = false,
                BackupSubject3PermissionFlag = false,
                BackupSubject4PermissionFlag = false,
                BackupSubject5PermissionFlag = false,
                BackupSubject6PermissionFlag = false,

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
                _subjectFlag.BackupSubject1PermissionFlag = subjectFlag.BackupSubject1PermissionFlag ?? _subjectFlag.BackupSubject1PermissionFlag;
                _subjectFlag.BackupSubject2PermissionFlag = subjectFlag.BackupSubject2PermissionFlag ?? _subjectFlag.BackupSubject2PermissionFlag;
                _subjectFlag.BackupSubject3PermissionFlag = subjectFlag.BackupSubject3PermissionFlag ?? _subjectFlag.BackupSubject3PermissionFlag;
                _subjectFlag.BackupSubject4PermissionFlag = subjectFlag.BackupSubject4PermissionFlag ?? _subjectFlag.BackupSubject4PermissionFlag;
                _subjectFlag.BackupSubject6PermissionFlag = subjectFlag.BackupSubject6PermissionFlag ?? _subjectFlag.BackupSubject6PermissionFlag;
                _context.SaveChanges();
            }
            return 1;
        }
    }
}
