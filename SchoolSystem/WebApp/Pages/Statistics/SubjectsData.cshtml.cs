using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Statistics;

public class SubjectsData(
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

    [BindProperty(SupportsGet = true)]
    public Domain.Subject Subject { get; set; } = default!;

    [BindProperty(SupportsGet = true)] public Domain.SubjectGiven SubjectGiven { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    
    public Guid SubjectId { get; set; }

    [BindProperty] public List<Domain.SubjectGiven> SubjectGivens { get; set; } = new();

    public void OnGet()
    {
        Subject = subjectRepository.LoadSubjects(SubjectId);
        SubjectGivens = subjectGivenRepository.LoadSubjectsGiven().Where(sg => sg.SubjectId == SubjectId).ToList();
    }
    
    public IActionResult OnPost(String method, Guid SubjectGivenId)
    {
        if (method == "addData")
        {
            Subject = subjectRepository.LoadSubjects(SubjectId);
            SubjectGiven.Subject = Subject;
            SubjectGiven.SubjectId = Subject.SubjectId;
            subjectGivenRepository.Save(SubjectGiven);
        }
        if (method == "data")
        {
            return RedirectToPage("/Statistics/SubjectGivenData", new {SubjectGivenId});
        }
        return Page();
    }
}