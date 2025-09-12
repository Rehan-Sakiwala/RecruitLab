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
        }
    }
}
