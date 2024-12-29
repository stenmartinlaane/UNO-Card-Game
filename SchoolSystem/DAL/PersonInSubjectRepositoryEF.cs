using Domain;

namespace DAL;

public class PersonInSubjectRepositoryEF : IPersonInSubjectRepository
{
    private readonly AppDbContext _ctx;

    public PersonInSubjectRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }
    
    public void Save(PersonInSubject personInSubject)
    {
        var personInSubjectInDb = _ctx.PersonInSubjects.FirstOrDefault(g => g.PersonId == personInSubject.PersonInSubjectId);
        if (personInSubjectInDb == null)
        {
            _ctx.PersonInSubjects.Add(personInSubject);
        }
        else
        {
            _ctx.Update(personInSubject);
        }
        _ctx.SaveChanges();
    }

    public PersonInSubject LoadPersonInSubject(Guid id)
    {
        return _ctx.PersonInSubjects.First(g => g.PersonInSubjectId == id);
    }

    public List<PersonInSubject> LoadPersonInSubject()
    {
        return _ctx.PersonInSubjects
            .OrderByDescending(g => g.GradeReceived)
            .ToList()!;
    }
}