using Microsoft.EntityFrameworkCore;
using recruitlab.server.Model.Entities.Server.Model.Entities;
using Server.Model.Entities;

namespace recruitlab.server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillCategory> SkillCategories { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<JobOpening> JobOpenings { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.CandidateProfile)
                .WithOne(c => c.User)
                .HasForeignKey<Candidate>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.EducationHistory)
                .WithOne(e => e.Candidate)
                .HasForeignKey(e => e.CandidateId);

            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.ExperienceHistory)
                .WithOne(e => e.Candidate)
                .HasForeignKey(e => e.CandidateId);

            modelBuilder.Entity<Candidate>()
                .HasMany(c => c.Documents)
                .WithOne(d => d.Candidate)
                .HasForeignKey(d => d.CandidateId);

            //Seedingg Data

            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Recruiter" },
                new Role { Id = 3, Name = "HR" },
                new Role { Id = 4, Name = "Interviewer" },
                new Role { Id = 5, Name = "Candidate" }
            };
            modelBuilder.Entity<Role>().HasData(roles);

            string passwordHash = BCrypt.Net.BCrypt.HashPassword("123");
            var users = new List<User>
            {
                new User {
                    Id = 1, FirstName = "Admin", LastName = "1",
                    Email = "admin@recruitlab.dev",
                    PasswordHash = passwordHash, RoleId = 1, Status = AccountStatus.Active
                },
                new User {
                    Id = 2, FirstName = "Recruiter", LastName = "1",
                    Email = "recruiter@recruitlab.dev",
                    PasswordHash = passwordHash, RoleId = 2, Status = AccountStatus.Active
                },
                new User {
                    Id = 3, FirstName = "HR", LastName = "1",
                    Email = "hr@recruitlab.dev",
                    PasswordHash = passwordHash, RoleId = 3, Status = AccountStatus.Active
                },
                new User {
                    Id = 4, FirstName = "Interviewer", LastName = "1",
                    Email = "interviewer@recruitlab.dev",
                    PasswordHash = passwordHash, RoleId = 4, Status = AccountStatus.Active
                }
            };
            modelBuilder.Entity<User>().HasData(users);
        }
    }
}
