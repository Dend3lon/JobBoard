using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DomainData.Entities;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;

namespace DomainData
{
    public class JobBoardContext : IdentityDbContext<IdentityUser>
    {
        public JobBoardContext(DbContextOptions<JobBoardContext> options)
            : base(options)
        {
        }

        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Application>()
            .HasOne(a => a.Resume)
            .WithMany(r => r.Applications)
            .HasForeignKey(a => a.ResumeId);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Vacancy)
                .WithMany(v => v.Applications)
                .HasForeignKey(a => a.VacancyId);
        }
    }
}
