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
}
