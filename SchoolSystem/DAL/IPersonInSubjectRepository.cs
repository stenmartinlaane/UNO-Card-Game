using Domain;

namespace DAL;

public interface IPersonInSubjectRepository
{
    void Save(PersonInSubject person);
    
    PersonInSubject LoadPersonInSubject(Guid id);

    List<PersonInSubject> LoadPersonInSubject();
}