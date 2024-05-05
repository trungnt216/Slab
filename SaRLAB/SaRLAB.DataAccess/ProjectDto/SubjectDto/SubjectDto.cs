using SaRLAB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaRLAB.DataAccess.ProjectDto.SubjectDto
{
    public class SubjectDto : ISubjectDto
    {
        private readonly ApplicationDbContext _context;

        public SubjectDto(ApplicationDbContext context)
        {
            _context = context;
        }

        public Subject delete(Subject subject)
        {
            throw new NotImplementedException();
        }

        public List<Subject> GetAll()
        {
            var user = _context.Subject.Select(value => new Subject
            {
                ID = value.ID,
                Rule = value.Rule,
                SubjectName = value.SubjectName
            });
            return user.ToList();
        }

        public Subject GetByID(int id)
        {
            var subject = _context.Subject.SingleOrDefault(item => item.ID == id);

            if (subject != null)
            {
                Subject _subject = new Subject
                {
                    ID = subject.ID,
                    Rule = subject.Rule,
                    SubjectName = subject.SubjectName
                };

                return _subject;
            }
            else
            {
                return null;
            }
        }

        public Subject GetByName(string name)
        {
            var subject = _context.Subject.SingleOrDefault(item => item.SubjectName == name);

            if (subject != null)
            {
                Subject _subject = new Subject
                {
                    ID = subject.ID,
                    Rule = subject.Rule,
                    SubjectName = subject.SubjectName
                };

                return _subject;
            }
            else
            {
                return null;
            }
        }

        public Subject insert(Subject subject)
        {
            var checkSubject = _context.Subject.SingleOrDefault(item => (item.SubjectName == subject.SubjectName));

            if (checkSubject != null)
            {
                return null;
            }

            var newSubject = new Subject
            {
                SubjectName = subject.SubjectName,
                Rule = subject.Rule,
            };

            _context.Subject.Add(newSubject);
            _context.SaveChanges();

            return newSubject;

        }

        public Subject update(Subject subject)
        {
            var _subject = _context.Subject.SingleOrDefault(item => (item.SubjectName == subject.SubjectName));

            if (_subject != null)
            {
                _subject.SubjectName = subject.SubjectName;
                _subject.Rule = subject.Rule;
            }


            return _subject;
        }
    }
}
