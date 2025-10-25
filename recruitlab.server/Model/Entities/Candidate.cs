using recruitlab.server.Model.Entities.Server.Model.Entities;

namespace Server.Model.Entities
{
    public class Candidate
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;

        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PortfolioUrl { get; set; }
        public CandidateSource Source { get; set; } = CandidateSource.Manual;
        public string? SourceDetails { get; set; }
        public DateTime? LastContactDate { get; set; }
        public string? LastContactNotes { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? AvailableFromDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
        public ICollection<Education> EducationHistory { get; set; } = new List<Education>();
        public ICollection<Experience> ExperienceHistory { get; set; } = new List<Experience>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }

    public enum CandidateSource
    {
        Manual = 1,
        CVUpload = 2,
        ExcelImport = 3,
        SelfRegistered = 4
    }
}