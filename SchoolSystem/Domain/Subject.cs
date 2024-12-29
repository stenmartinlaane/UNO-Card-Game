using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain;

public class Subject
{
    [Key]
    public Guid SubjectId { set; get; }
    
    [NotNull]
    [MaxLength(128)]
    [MinLength(1)]
    public string? SubjectName { get; set; }
    
    [NotNull]
    [MaxLength(128)]
    [MinLength(1)]
    public string? SubjectCode { get; set; }
    
    [NotNull]
    public int? ECTSForSubject { get; set; }
    
    [NotNull]
    [MaxLength(128)]
    [MinLength(1)]
    public string? SubjectDescription { get; set; }

    [NotNull]
    public List<SubjectGiven>? SubjectsGivens { get; set; }
}