using Domain;

namespace DAL;

public class SubjectRepositoryEF : ISubjectRepository
{
    private readonly AppDbContext _ctx;

    public SubjectRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }
    
    public void Save(Subject subject)
    {
        var subjectInDb = _ctx.Persons.FirstOrDefault(g => g.PersonId == subject.SubjectId);
        if (subjectInDb == null)
        {
            _ctx.Subjects.Add(subject);
        }
        else
        {
            _ctx.Update(subject);
        }
        _ctx.SaveChanges();
    }

    public Subject LoadSubjects(Guid id)
    {
        return _ctx.Subjects.First(g => g.SubjectId == id);
    }

    public List<Subject> LoadSubjects()
    {
        return _ctx.Subjects
            .OrderByDescending(g => g.SubjectName)
            .ToList()!;
    }
}