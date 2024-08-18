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
                SubjectName = value.SubjectName,
                Type = value.Type,
                VideoBackGround = value.VideoBackGround,
                MarkName = value.MarkName,
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
                    SubjectName = subject.SubjectName,
                    Type = subject.Type,
                    VideoBackGround = subject.VideoBackGround,
                    MarkName = subject.MarkName,
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
                    SubjectName = subject.SubjectName,
                    Type = subject.Type,
                    VideoBackGround = subject.VideoBackGround,
                    MarkName = subject.MarkName
                };

                return _subject;
            }
            else
            {
                return null;
            }
        }

        public Subject GetByTypeSchool(int type, int schoolId)
        {
            var subject = _context.Subjects.SingleOrDefault(item => (item.Type == type && item.SchoolId == schoolId));

            if (subject != null)
            {
                Subject _subject = new Subject
                {
                    ID = subject.ID,
                    Rule = subject.Rule,
                    SubjectName = subject.SubjectName,
                    Type = subject.Type,
                    SchoolId = subject.SchoolId,
                    VideoBackGround = subject.VideoBackGround,
                    MarkName = subject.MarkName

                };

                return _subject;
            }
            else
            {
                return null;
            }
        }

        public Subject GetSubjectBySchool(int schoolId)
        {
            var subject = _context.Subjects
                .SingleOrDefault(item => item.SchoolId == schoolId);
            return subject != null ? new Subject
            {
                ID = subject.ID,
                Rule = subject.Rule,
                SubjectName = subject.SubjectName,
                SchoolId = subject.SchoolId,
                Type = subject.Type,
                VideoBackGround = subject.VideoBackGround,
                MarkName = subject.MarkName,
            } : null;
        }

        public Subject GetSubjectBySchoolAndType(int schoolId, int type)
        {
            var subject = _context.Subjects.SingleOrDefault(item => (item.Type == type && item.SchoolId == schoolId));

            if (subject != null)
            {
                Subject _subject = new Subject
                {
                    ID = subject.ID,
                    Rule = subject.Rule,
                    SubjectName = subject.SubjectName,
                    Type = subject.Type,
                    VideoBackGround = subject.VideoBackGround,
                    MarkName = subject.MarkName

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

        public void InsertSubjects(List<Subject> subjects)
        {
            _context.Subjects.AddRange(subjects);
            _context.SaveChanges();
        }

        public Subject Update(Subject subject)
        {
            var _subject = _context.Subjects.SingleOrDefault(item => (item.Type == subject.Type && item.SchoolId == subject.SchoolId));

            if (_subject != null)
            {
                _subject.SubjectName = subject.SubjectName ?? _subject.SubjectName;
                _subject.Rule = subject.Rule ?? _subject.Rule;
                _subject.VideoBackGround = subject.VideoBackGround ?? _subject.VideoBackGround;
                _subject.MarkName = subject.MarkName ?? _subject.MarkName;
                _context.SaveChanges();
            }
            return _subject;
        }

        List<Subject> ISubjectDto.GetSubjectBySchool(int schoolId)
        {
            return _context.Subjects.Where(s => s.SchoolId == schoolId).ToList();
        }
    }
}
