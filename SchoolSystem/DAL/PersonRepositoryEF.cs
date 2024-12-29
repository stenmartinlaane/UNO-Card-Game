using Domain;

namespace DAL;

public class PersonRepositoryEF : IPersonRepository
{
    private readonly AppDbContext _ctx;

    public PersonRepositoryEF(AppDbContext ctx)
    {
        _ctx = ctx;
    }
    
    public void Save(Person person)
    {
        var personInDb = _ctx.Persons.FirstOrDefault(g => g.PersonId == person.PersonId);
        if (personInDb == null)
        {
            _ctx.Persons.Add(person);
        }
        else
        {
            _ctx.Update(person);
        }
        _ctx.SaveChanges();
    }

    public Person LoadPerson(Guid id)
    {
        return _ctx.Persons.First(g => g.PersonId == id);
    }

    public List<Person> LoadPersons()
    {
        return _ctx.Persons
            .OrderByDescending(g => g.FirstName)
            .ToList()!;
    }
}