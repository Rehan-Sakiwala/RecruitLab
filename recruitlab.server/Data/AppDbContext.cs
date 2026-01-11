using Microsoft.EntityFrameworkCore;
using recruitlab.server.Model.Entities;
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

        public DbSet<Application> Applications { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<InterviewAssignment> InterviewAssignments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<SkillRating> SkillRatings { get; set; }
        public DbSet<OfferLetter> OfferLetters { get; set; }


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

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Candidate)
                .WithMany()
                .HasForeignKey(a => a.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Application -> Job (Restrict Delete: Don't delete history if Job is deleted)
            modelBuilder.Entity<Application>()
                .HasOne(a => a.JobOpening)
                .WithMany()
                .HasForeignKey(a => a.JobOpeningId)
                .OnDelete(DeleteBehavior.Restrict);

            // Application -> Reviewer (User) (Restrict: Prevent cycles)
            modelBuilder.Entity<Application>()
                .HasOne(a => a.AssignedReviewer)
                .WithMany()
                .HasForeignKey(a => a.AssignedReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Interview -> Application
            modelBuilder.Entity<Interview>()
                .HasOne(i => i.Application)
                .WithMany(a => a.Interviews)
                .HasForeignKey(i => i.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // InterviewAssignment -> Interview
            modelBuilder.Entity<InterviewAssignment>()
                .HasOne(ia => ia.Interview)
                .WithMany(i => i.Assignments)
                .HasForeignKey(ia => ia.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // InterviewAssignment -> Interviewer (User) (Restrict)
            modelBuilder.Entity<InterviewAssignment>()
                .HasOne(ia => ia.Interviewer)
                .WithMany()
                .HasForeignKey(ia => ia.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback -> Interview
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Interview)
                .WithMany(i => i.Feedbacks)
                .HasForeignKey(f => f.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // Feedback -> Interviewer (User) (Restrict)
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Interviewer)
                .WithMany()
                .HasForeignKey(f => f.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // SkillRating -> Feedback
            modelBuilder.Entity<SkillRating>()
                .HasOne(sr => sr.Feedback)
                .WithMany(f => f.SkillRatings)
                .HasForeignKey(sr => sr.FeedbackId)
                .OnDelete(DeleteBehavior.Cascade);

            // SkillRating -> Skill (Restrict)
            modelBuilder.Entity<SkillRating>()
                .HasOne(sr => sr.Skill)
                .WithMany()
                .HasForeignKey(sr => sr.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

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
