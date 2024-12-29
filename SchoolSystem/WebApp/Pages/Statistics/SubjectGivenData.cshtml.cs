using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Pages.Statistics;

public class SubjectGivenData(
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

    [BindProperty(SupportsGet = true)] public Domain.PersonInSubject PersonInSubject { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public Domain.Person Person { get; set; } = default!;
    [BindProperty(SupportsGet = true)] public List<Domain.Person> Persons { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)] public List<SelectListItem> AllPersons { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public Domain.SubjectGiven SubjectGiven { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public Guid SubjectGivenId { get; set; } = default!;
    
    public void OnGet()
    {
        
        AllPersons = personRepository.LoadPersons().Select(p => new SelectListItem()
        {
            Value = p.PersonId.ToString(),
            Text = p.FirstName + " " + p.LastName
        }).ToList();
        Persons = personInSubjectRepository.LoadPersonInSubject().Where(ps => ps.SubjectGivenId == SubjectGivenId)
            .Select(ps => ps.Person).ToList();
    }
    
    public IActionResult OnPost(String method)
    {
        Console.WriteLine("id here");
        Console.WriteLine(SubjectGivenId);
        SubjectGiven = subjectGivenRepository.LoadSubjectGiven(SubjectGivenId);
        Console.WriteLine("past database");
        Console.WriteLine(SubjectGiven.SubjectGivenId);
        PersonInSubject.SubjectGiven = SubjectGiven;
        PersonInSubject.SubjectGivenId = SubjectGiven.SubjectId;
        PersonInSubject.Person = Person;
        PersonInSubject.PersonId = Person.PersonId;
        PersonInSubject.DeclaredSubject = DateTime.Now;
        PersonInSubject.FinishedSubject = DateTime.Now;
        SubjectGivenId = SubjectGiven.SubjectId;
        Console.WriteLine("DATA HERE");
        Console.WriteLine(PersonInSubject.PersonInSubjectId);
        Console.WriteLine(PersonInSubject.PersonId);
        Console.WriteLine(PersonInSubject.RoleInSubject);
        Console.WriteLine(PersonInSubject.GradeReceived);
        //personInSubjectRepository.Save(PersonInSubject);
        return RedirectToPage("/Statistics/SubjectGivenData", new {SubjectGivenId});
    }
}