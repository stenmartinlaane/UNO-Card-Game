using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; } = default!;
    public DbSet<Subject> Subjects { get; set; } = default!;
    public DbSet<SubjectGiven> SubjectGiven { get; set; } = default!;
    public DbSet<PersonInSubject> PersonInSubjects { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}