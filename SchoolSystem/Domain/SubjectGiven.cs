using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain;

public class SubjectGiven
{
    [Key]
    public Guid SubjectGivenId { get; set; }
    
    public Semester InSemester { get; set; }

    [NotNull]
    public int Year { get; set; } = 0;
    
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }
}