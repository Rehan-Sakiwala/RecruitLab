namespace Server.Model.DTO
{
    public class CandidateJobMatchDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public int JobOpeningId { get; set; }
        public string JobTitle { get; set; }
        public string JobDepartment { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public decimal MatchScore { get; set; }
        public string? MatchDetails { get; set; }
        public string? Notes { get; set; }
        public DateTime MatchedAt { get; set; }
        public DateTime? StatusUpdatedAt { get; set; }
        public int? MatchedByUserId { get; set; }
        public string? MatchedByUserEmail { get; set; }
        public DateTime? InterviewScheduledAt { get; set; }
        public string? InterviewNotes { get; set; }
        public decimal? OfferedSalary { get; set; }
        public DateTime? OfferDate { get; set; }
        public DateTime? ResponseDeadline { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCandidateJobMatchDto
    {
        public int CandidateId { get; set; }
        public int JobOpeningId { get; set; }
        public int Type { get; set; } = 2; // Manual by default
        public string? Notes { get; set; }
        public DateTime? InterviewScheduledAt { get; set; }
    }

    public class UpdateCandidateJobMatchDto
    {
        public int Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? InterviewScheduledAt { get; set; }
        public string? InterviewNotes { get; set; }
        public decimal? OfferedSalary { get; set; }
        public DateTime? OfferDate { get; set; }
        public DateTime? ResponseDeadline { get; set; }
        public bool IsActive { get; set; }
    }

    public class CandidateJobMatchSearchDto
    {
        public int? CandidateId { get; set; }
        public int? JobOpeningId { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public decimal? MinMatchScore { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? MatchedFromDate { get; set; }
        public DateTime? MatchedToDate { get; set; }
    }

    public class JobMatchRecommendationDto
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateEmail { get; set; }
        public decimal MatchScore { get; set; }
        public string MatchReason { get; set; }
        public List<SkillMatchDto> SkillMatches { get; set; } = new List<SkillMatchDto>();
        public List<SkillMismatchDto> SkillMismatches { get; set; } = new List<SkillMismatchDto>();
    }

    public class SkillMatchDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int CandidateLevel { get; set; }
        public string CandidateLevelName { get; set; }
        public int? CandidateYearsOfExperience { get; set; }
        public int JobRequirementType { get; set; }
        public string JobRequirementTypeName { get; set; }
        public int? JobYearsOfExperience { get; set; }
        public bool IsMatch { get; set; }
    }

    public class SkillMismatchDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int JobRequirementType { get; set; }
        public string JobRequirementTypeName { get; set; }
        public int? JobYearsOfExperience { get; set; }
        public string MismatchReason { get; set; }
    }
}
