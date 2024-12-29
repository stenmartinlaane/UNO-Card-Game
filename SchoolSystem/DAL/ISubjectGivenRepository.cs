using Domain;

namespace DAL;

public interface ISubjectGivenRepository
{
    void Save(SubjectGiven person);
    
    SubjectGiven LoadSubjectGiven(Guid id);

    List<SubjectGiven> LoadSubjectsGiven();
}