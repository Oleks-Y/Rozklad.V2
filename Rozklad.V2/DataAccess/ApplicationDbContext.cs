using Microsoft.EntityFrameworkCore;
using Rozklad.V2.Entities;

namespace Rozklad.V2.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext():base()
        {
            Database.EnsureCreated();
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Group> Groups { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.HasDefaultSchema("v2");
        //     base.OnModelCreating(modelBuilder);
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("User ID =postgres;Password=postgres;Server=localhost;Port=5432;Database=Rozklad;Integrated Security=true;Pooling=true;");
        }
    }
}