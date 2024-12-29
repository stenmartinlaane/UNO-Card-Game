using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Search;

public class Subjects : PageModel
{

    public List<Domain.Subject> Subjectz= new();
    private readonly ISubjectRepository _subjectRepository;

    public Subjects(ISubjectRepository subjectRepository, DAL.AppDbContext context)
    {
        _subjectRepository = subjectRepository;
    }

    [BindProperty]
    public Domain.Subject Subject { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public bool Add { get; set; } = false;
    
    public void OnGet()
    {
        Subjectz = _subjectRepository.LoadSubjects();
    }

    public IActionResult OnPost(String method, Guid SubjectId)
    {
        if (method == "addSubject")
        {
            _subjectRepository.Save(Subject);
            return RedirectToPage("/Search/Subjects", new { Add });
        }

        if (method == "addSubjectFrom")
        {
            Add = true;
            return RedirectToPage("/Search/Subjects", new { Add });
        }

        if (method == "subjectData")
        {
            return RedirectToPage("/Statistics/SubjectsData", new {SubjectId});
        }

        return Page();
    }
}