using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain;

public class Person
{
    [Key]
    public Guid PersonId { set; get; }

    [NotNull]
    [MaxLength(128)]
    [MinLength(1)]
    public string? FirstName { get; set; }
    
    [NotNull]
    [MaxLength(128)]
    [MinLength(1)]
    public string? LastName { get; set; }
    
    [NotNull]
    [MaxLength(256)]
    [MinLength(1)]
    public string? StudentCode { get; set; }
    
    [NotNull]
    public List<PersonInSubject>? PersonInSubjects { get; set; }
}