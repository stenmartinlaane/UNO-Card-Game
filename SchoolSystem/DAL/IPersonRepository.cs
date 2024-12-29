using Domain;

namespace DAL;

public interface IPersonRepository
{
    void Save(Person person);
    
    Person LoadPerson(Guid id);

    List<Person> LoadPersons();
}