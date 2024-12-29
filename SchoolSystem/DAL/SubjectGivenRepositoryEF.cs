using Domain;

namespace DAL;

public class SubjectGivenRepositoryEF : ISubjectGivenRepository
{
    private readonly AppDbContext _ctx;

    public SubjectGivenRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }
    
    public void Save(SubjectGiven subjectGiven)
    {
        var subjectGivenInDb = _ctx.SubjectGiven.FirstOrDefault(g => g.SubjectGivenId == subjectGiven.SubjectGivenId);
        if (subjectGivenInDb == null)
        {
            _ctx.SubjectGiven.Add(subjectGiven);
        }
        else
        {
            _ctx.Update(subjectGiven);
        }
        _ctx.SaveChanges();
    }

    public SubjectGiven LoadSubjectGiven(Guid id)
    {
        return _ctx.SubjectGiven.First(g => g.SubjectGivenId == id);
    }

    public List<SubjectGiven> LoadSubjectsGiven()
    {
        return _ctx.SubjectGiven
            .OrderByDescending(g => g.Year)
            .ToList()!;
    }
}