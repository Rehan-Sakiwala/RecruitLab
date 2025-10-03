using Microsoft.EntityFrameworkCore;
using Server.Model.Entities;

namespace recruitlab.server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<JobOpening> JobOpenings { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillCategory> SkillCategories { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<CandidateCV> CandidateCVs { get; set; }
        public DbSet<CandidateJobMatch> CandidateJobMatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JobOpening>()
                .HasOne(j => j.CreatedByUser)
                .WithMany()
                .HasForeignKey(j => j.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Skill>()
                .HasOne(s => s.SkillCategory)
                .WithMany(sc => sc.Skills)
                .HasForeignKey(s => s.SkillCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.JobOpening)
                .WithMany(j => j.JobSkills)
                .HasForeignKey(js => js.JobOpeningId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(js => js.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobSkill>()
                .HasIndex(js => new { js.JobOpeningId, js.SkillId })
                .IsUnique();

            modelBuilder.Entity<JobOpening>()
                .Property(j => j.Status)
                .HasConversion<int>();

            modelBuilder.Entity<JobSkill>()
                .Property(js => js.RequirementType)
                .HasConversion<int>();

            // Candidate
            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.CreatedByUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidate>()
                .Property(c => c.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Candidate>()
                .Property(c => c.Source)
                .HasConversion<int>();

            // CandidateSkill
            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.Candidate)
                .WithMany(c => c.CandidateSkills)
                .HasForeignKey(cs => cs.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.Skill)
                .WithMany()
                .HasForeignKey(cs => cs.SkillId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidateSkill>()
                .HasIndex(cs => new { cs.CandidateId, cs.SkillId })
                .IsUnique();

            modelBuilder.Entity<CandidateSkill>()
                .Property(cs => cs.Level)
                .HasConversion<int>();

            // CandidateCV
            modelBuilder.Entity<CandidateCV>()
                .HasOne(cv => cv.Candidate)
                .WithMany(c => c.CandidateCVs)
                .HasForeignKey(cv => cv.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateCV>()
                .HasOne(cv => cv.UploadedByUser)
                .WithMany()
                .HasForeignKey(cv => cv.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidateCV>()
                .Property(cv => cv.Type)
                .HasConversion<int>();

            modelBuilder.Entity<CandidateCV>()
                .Property(cv => cv.Status)
                .HasConversion<int>();

            // CandidateJobMatch
            modelBuilder.Entity<CandidateJobMatch>()
                .HasOne(cjm => cjm.Candidate)
                .WithMany(c => c.CandidateJobMatches)
                .HasForeignKey(cjm => cjm.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateJobMatch>()
                .HasOne(cjm => cjm.JobOpening)
                .WithMany()
                .HasForeignKey(cjm => cjm.JobOpeningId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CandidateJobMatch>()
                .HasOne(cjm => cjm.MatchedByUser)
                .WithMany()
                .HasForeignKey(cjm => cjm.MatchedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CandidateJobMatch>()
                .Property(cjm => cjm.Status)
                .HasConversion<int>();

            modelBuilder.Entity<CandidateJobMatch>()
                .Property(cjm => cjm.Type)
                .HasConversion<int>();

            modelBuilder.Entity<CandidateJobMatch>()
                .HasIndex(cjm => new { cjm.CandidateId, cjm.JobOpeningId })
                .IsUnique();
        }
    }
}
