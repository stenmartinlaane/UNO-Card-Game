using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Domain;

public class PersonInSubject
{
    [Key]
    public Guid PersonInSubjectId { get; set; }
    
    public DateTime? DeclaredSubject { get; set; }
    
    public DateTime? FinishedSubject { get; set; }
    
    public Grade GradeReceived { get; set; } = Grade.NotAdded;
    
    public RoleInSubject RoleInSubject { get; set; }
    
    public Guid PersonId { get; set; }
    [NotNull]
    public Person? Person { get; set; }
    
    public Guid SubjectGivenId { get; set; }
    [NotNull]
    public SubjectGiven? SubjectGiven { get; set; }
}