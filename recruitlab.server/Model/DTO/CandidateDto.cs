namespace Server.Model.DTO
{
    public class CandidateDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CurrentPosition { get; set; }
        public string? CurrentCompany { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? ExperienceSummary { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PortfolioUrl { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Source { get; set; }
        public string SourceName { get; set; }
        public string? SourceDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserEmail { get; set; }
        public DateTime? LastContactDate { get; set; }
        public string? LastContactNotes { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? AvailableFromDate { get; set; }
        public List<CandidateSkillDto> CandidateSkills { get; set; } = new List<CandidateSkillDto>();
        public List<CandidateCVDto> CandidateCVs { get; set; } = new List<CandidateCVDto>();
        public List<CandidateJobMatchDto> CandidateJobMatches { get; set; } = new List<CandidateJobMatchDto>();
    }

    public class CreateCandidateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CurrentPosition { get; set; }
        public string? CurrentCompany { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? ExperienceSummary { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PortfolioUrl { get; set; }
        public int Source { get; set; } = 1;
        public string? SourceDetails { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? AvailableFromDate { get; set; }
        public List<CreateCandidateSkillDto> CandidateSkills { get; set; } = new List<CreateCandidateSkillDto>();
    }

    public class UpdateCandidateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CurrentPosition { get; set; }
        public string? CurrentCompany { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? ExperienceSummary { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PortfolioUrl { get; set; }
        public int Status { get; set; }
        public int Source { get; set; }
        public string? SourceDetails { get; set; }
        public DateTime? LastContactDate { get; set; }
        public string? LastContactNotes { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? AvailableFromDate { get; set; }
    }

    public class BulkCreateCandidateDto
    {
        public string Source { get; set; } = "Excel Import";
        public string? SourceDetails { get; set; }
        public List<CreateCandidateDto> Candidates { get; set; } = new List<CreateCandidateDto>();
    }
}
