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
        public SubjectFlag getSubjectFlagByUserEmail(string userEmail)
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
                    BackupSubject7MarkFlag = subjectFlag.BackupSubject7MarkFlag,
                    BackupSubject7PermissionFlag = subjectFlag.BackupSubject7PermissionFlag,
                    BackupSubject8MarkFlag = subjectFlag.BackupSubject8MarkFlag,
                    BackupSubject8PermissionFlag = subjectFlag.BackupSubject8PermissionFlag,
                    BackupSubject9MarkFlag = subjectFlag.BackupSubject9MarkFlag,
                    BackupSubject9PermissionFlag = subjectFlag.BackupSubject9PermissionFlag,
                    BackupSubject10MarkFlag = subjectFlag.BackupSubject10MarkFlag,
                    BackupSubject10PermissionFlag = subjectFlag.BackupSubject10PermissionFlag,
                    BackupSubject11MarkFlag = subjectFlag.BackupSubject11MarkFlag,
                    BackupSubject11PermissionFlag = subjectFlag.BackupSubject11PermissionFlag,
                    BackupSubject12MarkFlag = subjectFlag.BackupSubject12MarkFlag,
                    BackupSubject12PermissionFlag = subjectFlag.BackupSubject12PermissionFlag,
                    BackupSubject13MarkFlag = subjectFlag.BackupSubject13MarkFlag,
                    BackupSubject13PermissionFlag = subjectFlag.BackupSubject13PermissionFlag,
                    BackupSubject14MarkFlag = subjectFlag.BackupSubject14MarkFlag,
                    BackupSubject14PermissionFlag = subjectFlag.BackupSubject14PermissionFlag,
                    BackupSubject15MarkFlag = subjectFlag.BackupSubject15MarkFlag,
                    BackupSubject15PermissionFlag = subjectFlag.BackupSubject15PermissionFlag,
                    BackupSubject16MarkFlag = subjectFlag.BackupSubject16MarkFlag,
                    BackupSubject16PermissionFlag = subjectFlag.BackupSubject16PermissionFlag,
                    BackupSubject17MarkFlag = subjectFlag.BackupSubject17MarkFlag,
                    BackupSubject17PermissionFlag = subjectFlag.BackupSubject17PermissionFlag,
                    BackupSubject18MarkFlag = subjectFlag.BackupSubject18MarkFlag,
                    BackupSubject18PermissionFlag = subjectFlag.BackupSubject18PermissionFlag,
                    BackupSubject19MarkFlag = subjectFlag.BackupSubject19MarkFlag,
                    BackupSubject19PermissionFlag = subjectFlag.BackupSubject19PermissionFlag,
                    BackupSubject20MarkFlag = subjectFlag.BackupSubject20MarkFlag,
                    BackupSubject20PermissionFlag = subjectFlag.BackupSubject20PermissionFlag,
                    BackupSubject21MarkFlag = subjectFlag.BackupSubject21MarkFlag,
                    BackupSubject21PermissionFlag = subjectFlag.BackupSubject21PermissionFlag,
                    BackupSubject22MarkFlag = subjectFlag.BackupSubject22MarkFlag,
                    BackupSubject22PermissionFlag = subjectFlag.BackupSubject22PermissionFlag,
                    BackupSubject23MarkFlag = subjectFlag.BackupSubject23MarkFlag,
                    BackupSubject23PermissionFlag = subjectFlag.BackupSubject23PermissionFlag,
                    BackupSubject24MarkFlag = subjectFlag.BackupSubject24MarkFlag,
                    BackupSubject24PermissionFlag = subjectFlag.BackupSubject24PermissionFlag,
                    BackupSubject25MarkFlag = subjectFlag.BackupSubject25MarkFlag,
                    BackupSubject25PermissionFlag = subjectFlag.BackupSubject25PermissionFlag,
                    BackupSubject26MarkFlag = subjectFlag.BackupSubject26MarkFlag,
                    BackupSubject26PermissionFlag = subjectFlag.BackupSubject26PermissionFlag,
                    BackupSubject27MarkFlag = subjectFlag.BackupSubject27MarkFlag,
                    BackupSubject27PermissionFlag = subjectFlag.BackupSubject27PermissionFlag,
                    BackupSubject28MarkFlag = subjectFlag.BackupSubject28MarkFlag,
                    BackupSubject28PermissionFlag = subjectFlag.BackupSubject28PermissionFlag,
                    BackupSubject29MarkFlag = subjectFlag.BackupSubject29MarkFlag,
                    BackupSubject29PermissionFlag = subjectFlag.BackupSubject29PermissionFlag,
                    BackupSubject30MarkFlag = subjectFlag.BackupSubject30MarkFlag,
                    BackupSubject30PermissionFlag = subjectFlag.BackupSubject30PermissionFlag,
                    BackupSubject31MarkFlag = subjectFlag.BackupSubject31MarkFlag,
                    BackupSubject31PermissionFlag = subjectFlag.BackupSubject31PermissionFlag,
                    BackupSubject32MarkFlag = subjectFlag.BackupSubject32MarkFlag,
                    BackupSubject32PermissionFlag = subjectFlag.BackupSubject32PermissionFlag
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
                BackupSubject7MarkFlag = false,
                BackupSubject8MarkFlag = false,
                BackupSubject9MarkFlag = false,
                BackupSubject10MarkFlag = false,
                BackupSubject11MarkFlag = false,
                BackupSubject12MarkFlag = false,
                BackupSubject13MarkFlag = false,
                BackupSubject14MarkFlag = false,
                BackupSubject15MarkFlag = false,
                BackupSubject16MarkFlag = false,
                BackupSubject17MarkFlag = false,
                BackupSubject18MarkFlag = false,
                BackupSubject19MarkFlag = false,
                BackupSubject20MarkFlag = false,
                BackupSubject21MarkFlag = false,
                BackupSubject22MarkFlag = false,
                BackupSubject23MarkFlag = false,
                BackupSubject24MarkFlag = false,
                BackupSubject25MarkFlag = false,
                BackupSubject26MarkFlag = false,
                BackupSubject27MarkFlag = false,
                BackupSubject28MarkFlag = false,
                BackupSubject29MarkFlag = false,
                BackupSubject30MarkFlag = false,
                BackupSubject31MarkFlag = false,
                BackupSubject32MarkFlag = false,
                BackupSubject1PermissionFlag = false,
                BackupSubject2PermissionFlag = false,
                BackupSubject3PermissionFlag = false,
                BackupSubject4PermissionFlag = false,
                BackupSubject5PermissionFlag = false,
                BackupSubject6PermissionFlag = false,
                BackupSubject7PermissionFlag = false,
                BackupSubject8PermissionFlag = false,
                BackupSubject9PermissionFlag = false,
                BackupSubject10PermissionFlag = false,
                BackupSubject11PermissionFlag = false,
                BackupSubject12PermissionFlag = false,
                BackupSubject13PermissionFlag = false,
                BackupSubject14PermissionFlag = false,
                BackupSubject15PermissionFlag = false,
                BackupSubject16PermissionFlag = false,
                BackupSubject17PermissionFlag = false,
                BackupSubject18PermissionFlag = false,
                BackupSubject19PermissionFlag = false,
                BackupSubject20PermissionFlag = false,
                BackupSubject21PermissionFlag = false,
                BackupSubject22PermissionFlag = false,
                BackupSubject23PermissionFlag = false,
                BackupSubject24PermissionFlag = false,
                BackupSubject25PermissionFlag = false,
                BackupSubject26PermissionFlag = false,
                BackupSubject27PermissionFlag = false,
                BackupSubject28PermissionFlag = false,
                BackupSubject29PermissionFlag = false,
                BackupSubject30PermissionFlag = false,
                BackupSubject31PermissionFlag = false,
                BackupSubject32PermissionFlag = false,

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
                _subjectFlag.BackupSubject5PermissionFlag = subjectFlag.BackupSubject5PermissionFlag ?? _subjectFlag.BackupSubject5PermissionFlag;
                _subjectFlag.BackupSubject6PermissionFlag = subjectFlag.BackupSubject6PermissionFlag ?? _subjectFlag.BackupSubject6PermissionFlag;
                _subjectFlag.BackupSubject7PermissionFlag = subjectFlag.BackupSubject7PermissionFlag ?? _subjectFlag.BackupSubject7PermissionFlag;
                _subjectFlag.BackupSubject8PermissionFlag = subjectFlag.BackupSubject8PermissionFlag ?? _subjectFlag.BackupSubject8PermissionFlag;
                _subjectFlag.BackupSubject9PermissionFlag = subjectFlag.BackupSubject9PermissionFlag ?? _subjectFlag.BackupSubject9PermissionFlag;
                _subjectFlag.BackupSubject10PermissionFlag = subjectFlag.BackupSubject10PermissionFlag ?? _subjectFlag.BackupSubject10PermissionFlag;
                _subjectFlag.BackupSubject11PermissionFlag = subjectFlag.BackupSubject11PermissionFlag ?? _subjectFlag.BackupSubject11PermissionFlag;
                _subjectFlag.BackupSubject12PermissionFlag = subjectFlag.BackupSubject12PermissionFlag ?? _subjectFlag.BackupSubject12PermissionFlag;
                _subjectFlag.BackupSubject13PermissionFlag = subjectFlag.BackupSubject13PermissionFlag ?? _subjectFlag.BackupSubject13PermissionFlag;
                _subjectFlag.BackupSubject14PermissionFlag = subjectFlag.BackupSubject14PermissionFlag ?? _subjectFlag.BackupSubject14PermissionFlag;
                _subjectFlag.BackupSubject15PermissionFlag = subjectFlag.BackupSubject15PermissionFlag ?? _subjectFlag.BackupSubject15PermissionFlag;
                _subjectFlag.BackupSubject16PermissionFlag = subjectFlag.BackupSubject16PermissionFlag ?? _subjectFlag.BackupSubject16PermissionFlag;
                _subjectFlag.BackupSubject17PermissionFlag = subjectFlag.BackupSubject17PermissionFlag ?? _subjectFlag.BackupSubject17PermissionFlag;
                _subjectFlag.BackupSubject18PermissionFlag = subjectFlag.BackupSubject18PermissionFlag ?? _subjectFlag.BackupSubject18PermissionFlag;
                _subjectFlag.BackupSubject19PermissionFlag = subjectFlag.BackupSubject19PermissionFlag ?? _subjectFlag.BackupSubject19PermissionFlag;
                _subjectFlag.BackupSubject20PermissionFlag = subjectFlag.BackupSubject20PermissionFlag ?? _subjectFlag.BackupSubject20PermissionFlag;
                _subjectFlag.BackupSubject21PermissionFlag = subjectFlag.BackupSubject21PermissionFlag ?? _subjectFlag.BackupSubject21PermissionFlag;
                _subjectFlag.BackupSubject22PermissionFlag = subjectFlag.BackupSubject22PermissionFlag ?? _subjectFlag.BackupSubject22PermissionFlag;
                _subjectFlag.BackupSubject23PermissionFlag = subjectFlag.BackupSubject23PermissionFlag ?? _subjectFlag.BackupSubject23PermissionFlag;
                _subjectFlag.BackupSubject24PermissionFlag = subjectFlag.BackupSubject24PermissionFlag ?? _subjectFlag.BackupSubject24PermissionFlag;
                _subjectFlag.BackupSubject25PermissionFlag = subjectFlag.BackupSubject25PermissionFlag ?? _subjectFlag.BackupSubject25PermissionFlag;
                _subjectFlag.BackupSubject26PermissionFlag = subjectFlag.BackupSubject26PermissionFlag ?? _subjectFlag.BackupSubject26PermissionFlag;
                _subjectFlag.BackupSubject27PermissionFlag = subjectFlag.BackupSubject27PermissionFlag ?? _subjectFlag.BackupSubject27PermissionFlag;
                _subjectFlag.BackupSubject28PermissionFlag = subjectFlag.BackupSubject28PermissionFlag ?? _subjectFlag.BackupSubject28PermissionFlag;
                _subjectFlag.BackupSubject29PermissionFlag = subjectFlag.BackupSubject29PermissionFlag ?? _subjectFlag.BackupSubject29PermissionFlag;
                _subjectFlag.BackupSubject30PermissionFlag = subjectFlag.BackupSubject30PermissionFlag ?? _subjectFlag.BackupSubject30PermissionFlag;
                _subjectFlag.BackupSubject31PermissionFlag = subjectFlag.BackupSubject31PermissionFlag ?? _subjectFlag.BackupSubject31PermissionFlag;
                _subjectFlag.BackupSubject32PermissionFlag = subjectFlag.BackupSubject32PermissionFlag ?? _subjectFlag.BackupSubject32PermissionFlag;
                _subjectFlag.BackupSubject1MarkFlag = subjectFlag.BackupSubject1MarkFlag ?? _subjectFlag.BackupSubject1MarkFlag;
                _subjectFlag.BackupSubject2MarkFlag = subjectFlag.BackupSubject2MarkFlag ?? _subjectFlag.BackupSubject2MarkFlag;
                _subjectFlag.BackupSubject3MarkFlag = subjectFlag.BackupSubject3MarkFlag ?? _subjectFlag.BackupSubject3MarkFlag;
                _subjectFlag.BackupSubject4MarkFlag = subjectFlag.BackupSubject4MarkFlag ?? _subjectFlag.BackupSubject4MarkFlag;
                _subjectFlag.BackupSubject5MarkFlag = subjectFlag.BackupSubject5MarkFlag ?? _subjectFlag.BackupSubject5MarkFlag;
                _subjectFlag.BackupSubject6MarkFlag = subjectFlag.BackupSubject6MarkFlag ?? _subjectFlag.BackupSubject6MarkFlag;
                _subjectFlag.BackupSubject7MarkFlag = subjectFlag.BackupSubject7MarkFlag ?? _subjectFlag.BackupSubject7MarkFlag;
                _subjectFlag.BackupSubject8MarkFlag = subjectFlag.BackupSubject8MarkFlag ?? _subjectFlag.BackupSubject8MarkFlag;
                _subjectFlag.BackupSubject9MarkFlag = subjectFlag.BackupSubject9MarkFlag ?? _subjectFlag.BackupSubject9MarkFlag;
                _subjectFlag.BackupSubject10MarkFlag = subjectFlag.BackupSubject10MarkFlag ?? _subjectFlag.BackupSubject10MarkFlag;
                _subjectFlag.BackupSubject11MarkFlag = subjectFlag.BackupSubject11MarkFlag ?? _subjectFlag.BackupSubject11MarkFlag;
                _subjectFlag.BackupSubject12MarkFlag = subjectFlag.BackupSubject12MarkFlag ?? _subjectFlag.BackupSubject12MarkFlag;
                _subjectFlag.BackupSubject13MarkFlag = subjectFlag.BackupSubject13MarkFlag ?? _subjectFlag.BackupSubject13MarkFlag;
                _subjectFlag.BackupSubject14MarkFlag = subjectFlag.BackupSubject14MarkFlag ?? _subjectFlag.BackupSubject14MarkFlag;
                _subjectFlag.BackupSubject15MarkFlag = subjectFlag.BackupSubject15MarkFlag ?? _subjectFlag.BackupSubject15MarkFlag;
                _subjectFlag.BackupSubject16MarkFlag = subjectFlag.BackupSubject16MarkFlag ?? _subjectFlag.BackupSubject16MarkFlag;
                _subjectFlag.BackupSubject17MarkFlag = subjectFlag.BackupSubject17MarkFlag ?? _subjectFlag.BackupSubject17MarkFlag;
                _subjectFlag.BackupSubject18MarkFlag = subjectFlag.BackupSubject18MarkFlag ?? _subjectFlag.BackupSubject18MarkFlag;
                _subjectFlag.BackupSubject19MarkFlag = subjectFlag.BackupSubject19MarkFlag ?? _subjectFlag.BackupSubject19MarkFlag;
                _subjectFlag.BackupSubject20MarkFlag = subjectFlag.BackupSubject20MarkFlag ?? _subjectFlag.BackupSubject20MarkFlag;
                _subjectFlag.BackupSubject21MarkFlag = subjectFlag.BackupSubject21MarkFlag ?? _subjectFlag.BackupSubject21MarkFlag;
                _subjectFlag.BackupSubject22MarkFlag = subjectFlag.BackupSubject22MarkFlag ?? _subjectFlag.BackupSubject22MarkFlag;
                _subjectFlag.BackupSubject23MarkFlag = subjectFlag.BackupSubject23MarkFlag ?? _subjectFlag.BackupSubject23MarkFlag;
                _subjectFlag.BackupSubject24MarkFlag = subjectFlag.BackupSubject24MarkFlag ?? _subjectFlag.BackupSubject24MarkFlag;
                _subjectFlag.BackupSubject25MarkFlag = subjectFlag.BackupSubject25MarkFlag ?? _subjectFlag.BackupSubject25MarkFlag;
                _subjectFlag.BackupSubject26MarkFlag = subjectFlag.BackupSubject26MarkFlag ?? _subjectFlag.BackupSubject26MarkFlag;
                _subjectFlag.BackupSubject27MarkFlag = subjectFlag.BackupSubject27MarkFlag ?? _subjectFlag.BackupSubject27MarkFlag;
                _subjectFlag.BackupSubject28MarkFlag = subjectFlag.BackupSubject28MarkFlag ?? _subjectFlag.BackupSubject28MarkFlag;
                _subjectFlag.BackupSubject29MarkFlag = subjectFlag.BackupSubject29MarkFlag ?? _subjectFlag.BackupSubject29MarkFlag;
                _subjectFlag.BackupSubject30MarkFlag = subjectFlag.BackupSubject30MarkFlag ?? _subjectFlag.BackupSubject30MarkFlag;
                _subjectFlag.BackupSubject31MarkFlag = subjectFlag.BackupSubject31MarkFlag ?? _subjectFlag.BackupSubject31MarkFlag;
                _subjectFlag.BackupSubject32MarkFlag = subjectFlag.BackupSubject32MarkFlag ?? _subjectFlag.BackupSubject32MarkFlag;
                _context.SaveChanges();
            }
            return 1;
        }
    }
}
