namespace Server.Model.Entities
{
    public enum MatchStatus
    {
        Pending = 1,
        Reviewing = 2,
        InterviewScheduled = 3,
        Interviewed = 4,
        Shortlisted = 5,
        Offered = 6,
        Hired = 7,
        Rejected = 8,
        Withdrawn = 9
    }

    public enum MatchType
    {
        Automatic = 1,
        Manual = 2,
        Recommended = 3
    }

    public class CandidateJobMatch
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int JobOpeningId { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.Pending;
        public MatchType Type { get; set; } = MatchType.Automatic;
        public decimal MatchScore { get; set; }
        public string? MatchDetails { get; set; } 
        public string? Notes { get; set; }
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StatusUpdatedAt { get; set; }
        public int? MatchedByUserId { get; set; } 
        public DateTime? InterviewScheduledAt { get; set; }
        public string? InterviewNotes { get; set; }
        public decimal? OfferedSalary { get; set; }
        public DateTime? OfferDate { get; set; }
        public DateTime? ResponseDeadline { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Candidate Candidate { get; set; }
        public JobOpening JobOpening { get; set; }
        public User? MatchedByUser { get; set; }
    }
}
