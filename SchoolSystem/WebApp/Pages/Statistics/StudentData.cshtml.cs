using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages.Statistics;

public class StudentData(
    IPersonRepository personRepository,
    DAL.AppDbContext context,
    ISubjectRepository subjectRepository,
    IPersonInSubjectRepository personInSubjectRepository,
    ISubjectGivenRepository subjectGivenRepository)
    : PageModel
{
    private readonly IPersonRepository _personRepository = personRepository;
    private readonly ISubjectRepository _subjectRepository = subjectRepository;
    private readonly IPersonInSubjectRepository _personInSubjectRepository = personInSubjectRepository;
    private readonly ISubjectGivenRepository _subjectGivenRepository = subjectGivenRepository;

    public Domain.Person Person { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public Guid PersonId { get; set; }

    public List<Domain.Subject> Subjects { get; set; } = new();
    
    public void OnGet()
    {
        Person = personRepository.LoadPerson(PersonId);
        
        List<Domain.PersonInSubject> personInSubjects =
           personInSubjectRepository.LoadPersonInSubject().Where(s => s.PersonId == Person.PersonId).ToList();
            
        List<Domain.SubjectGiven> subjectGivens =
            subjectGivenRepository.LoadSubjectsGiven()
               .Where(
                   sg => personInSubjects.Select(s => s.SubjectGivenId)
                        .Contains(sg.SubjectGivenId)
                    ).ToList();
        Subjects = subjectRepository.LoadSubjects().Where(
            sub => subjectGivens.Select(sg => sg.SubjectId)
                .Contains(sub.SubjectId)
        ).ToList();
    }
}