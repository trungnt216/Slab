using SaRLAB.Models.Entity;

namespace SaRLAB.DataAccess.Service.SubjectDto
{
    public class SubjectDto : ISubjectDto
    {
        private readonly ApplicationDbContext _context;

        public SubjectDto(ApplicationDbContext context)
        {
            _context = context;
        }

        public void DeleteById(int id)
        {
            var subjectToDelete = _context.Subjects.Find(id);
            if (subjectToDelete != null)
            {
                _context.Subjects.Remove(subjectToDelete);
                _context.SaveChanges();
            }
        }

        public List<Subject> GetAll()
        {
            var user = _context.Subjects.Select(value => new Subject
            {
                ID = value.ID,
                Rule = value.Rule,
                SubjectName = value.SubjectName
            });
            return user.ToList();
        }

        public Subject GetByID(int id)
        {
            var subject = _context.Subjects.SingleOrDefault(item => item.ID == id);

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
            var subject = _context.Subjects.SingleOrDefault(item => item.SubjectName == name);

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

        public Subject Insert(Subject subject)
        {
            var checkSubject = _context.Subjects.SingleOrDefault(item => (item.SubjectName == subject.SubjectName));

            if (checkSubject != null)
            {
                return null;
            }

            var newSubject = new Subject
            {
                SubjectName = subject.SubjectName,
                Rule = subject.Rule,
            };

            _context.Subjects.Add(newSubject);
            _context.SaveChanges();

            return newSubject;

        }

        public Subject Update(Subject subject)
        {
            var _subject = _context.Subjects.SingleOrDefault(item => (item.SubjectName == subject.SubjectName));

            if (_subject != null)
            {
                _subject.SubjectName = subject.SubjectName;
                _subject.Rule = subject.Rule;
                _context.SaveChanges();
            }


            return _subject;
        }

    }
}
