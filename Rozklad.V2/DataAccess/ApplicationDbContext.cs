using Microsoft.EntityFrameworkCore;
using Rozklad.V2.Entities;

namespace Rozklad.V2.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Student>().OwnsOne<NotificationsSettings>(s=>s.NotificationsSettings, p =>
        //     {
        //         p.WithOwner().HasForeignKey("StudentId");
        //     });
        // }

        public DbSet<Student> Students { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<DisabledSubject> DisabledSubjects { get; set; }

        public DbSet<NotificationsSettings> NotificationsSettings { get; set; }

        public DbSet<TelegramData> TelegramData { get; set; }

        public DbSet<MutedSubject> MutedSubjects { get; set; }
        
        public DbSet<GoogleData> GoogleData { get; set; }
    }
}