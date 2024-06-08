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
        public SubjectFlag getSubjectFlagByUserId(int userId)
        {
            var subjectFlag = _context.SubjectFlags.SingleOrDefault(item => item.UserId == userId);

            if (subjectFlag != null)
            {
                SubjectFlag _subjectFlag = new SubjectFlag
                {
                    ID = subjectFlag.ID,
                    UserId = subjectFlag.UserId,
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

        public int InsertSubjectFlag(SubjectFlag subjectFlag)
        {
            var checkSubjectFlag = _context.SubjectFlags.SingleOrDefault(item => (item.UserId == subjectFlag.UserId));

            if (checkSubjectFlag != null)
            {
                return 0;
            }

            var newSubjectFlag = new SubjectFlag
            {
                UserId = subjectFlag.UserId,
                MathPermissionFlag = subjectFlag.MathPermissionFlag,
                MathMarkFlag = subjectFlag.MathMarkFlag,
                PhysicPermissionFlag = subjectFlag.PhysicPermissionFlag,
                PhysicMarkFlag = subjectFlag.PhysicMarkFlag,
                BiologyPermissionFlag = subjectFlag.BiologyPermissionFlag,
                BiologyMarkFlag = subjectFlag.BiologyMarkFlag,
                ChemistryPermissionFlag = subjectFlag.ChemistryPermissionFlag,
                ChemistryMarkFlag = subjectFlag.ChemistryMarkFlag,
            };

            _context.SubjectFlags.Add(newSubjectFlag);
            _context.SaveChanges();

            return 1;
        }

        public int updateSubjectFlag(int userId, SubjectFlag subjectFlag)
        {
            var _subjectFlag = _context.SubjectFlags.SingleOrDefault(item => (item.UserId == subjectFlag.UserId));

            if (_subjectFlag != null)
            {
                _subjectFlag.UserId = subjectFlag.UserId;
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
