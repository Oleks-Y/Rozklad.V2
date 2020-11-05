using Microsoft.EntityFrameworkCore;
using Rozklad.V2.Entities;

namespace Rozklad.V2.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        
        public DbSet<Lesson> Lessons { get; set; }
        
        public DbSet<Subject> Subjects { get; set; }

        public Group Groups { get; set; }
    }
}