using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Search;

public class Students(IPersonRepository personRepository, DAL.AppDbContext context) : PageModel
{
    public List<Domain.Person> Persons { get; set; } = new();

    [BindProperty] public Domain.Person Person { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public bool Add { get; set; } = false;
    
    public void OnGet()
    {
        Persons = personRepository.LoadPersons();
    }

    public IActionResult OnPost(Guid PersonId, String method)
    {
        if (method == "addStudent")
        {
            personRepository.Save(Person);
        } else if (method == "addStudentFrom")
        {
            Add = true;
            return RedirectToPage("/Search/Students", new { Add });
        } else if (method == "data")
        {
            return RedirectToPage("/Statistics/StudentData", new {PersonId});
        }

        return RedirectToPage("/Statistics/StudentData");
    }
}