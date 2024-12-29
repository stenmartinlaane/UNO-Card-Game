using Domain;

namespace DAL;

public interface ISubjectRepository
{
    void Save(Subject subject);
    
    Subject LoadSubjects(Guid id);

    List<Subject> LoadSubjects();
}