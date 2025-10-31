using Server.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace recruitlab.server.Model.DTO
{
    public class CandidateListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? CurrentPosition { get; set; }
        public string? CurrentCompany { get; set; }
        public bool IsAvailable { get; set; }
        public string Source { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }

    public class CandidateProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PortfolioUrl { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }
        public string Source { get; set; } = string.Empty;
        public string? SourceDetails { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? AvailableFromDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<CandidateSkillDto> CandidateSkills { get; set; } = new List<CandidateSkillDto>();
        public List<EducationDto> EducationHistory { get; set; } = new List<EducationDto>();
        public List<ExperienceDto> ExperienceHistory { get; set; } = new List<ExperienceDto>();
        public List<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
    }

    public class CandidateSkillDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillCategoryName { get; set; } = string.Empty;
        public int Level { get; set; }
        public string LevelName { get; set; } = string.Empty;
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsVerified { get; set; }
    }

    public class EducationDto
    {
        public int Id { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Notes { get; set; }
    }

    public class ExperienceDto
    {
        public int Id { get; set; }
        public string Position { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Responsibilities { get; set; }
    }

    public class DocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class CreateCandidateSkillDto
    {
        [Required]
        public int SkillId { get; set; }
        [Required]
        public int Level { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsVerified { get; set; }
    }

    public class UpdateCandidateSkillDto
    {
        [Required]
        public int Level { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsVerified { get; set; }
    }

    public class CreateEducationDto
    {
        [Required]
        public string SchoolName { get; set; } = string.Empty;
        [Required]
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public string? Notes { get; set; }
    }

    public class UpdateEducationDto
    {
        [Required]
        public string SchoolName { get; set; } = string.Empty;
        [Required]
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public string? Notes { get; set; }
    }

    public class CreateExperienceDto
    {
        [Required]
        public string Position { get; set; } = string.Empty;
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public string? Responsibilities { get; set; }
    }

    public class UpdateExperienceDto
    {
        [Required]
        public string Position { get; set; } = string.Empty;
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public string? Responsibilities { get; set; }
    }

    public class UploadCvDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
