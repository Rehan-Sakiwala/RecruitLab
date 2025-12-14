using System.ComponentModel.DataAnnotations;

namespace recruitlab.server.Model.DTO
{
    public class CandidateApplyDto
    {
        [Required]
        public int JobOpeningId { get; set; }
    }

    public class ApplicationSummaryDto
    {
        public int Id { get; set; }
        public int JobOpeningId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
    }

    public class ApplicationRecruiterViewDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;

        public string Stage { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }

        public int? AssignedReviewerId { get; set; }
        public string ReviewerName { get; set; } = "Unassigned";
    }
}
