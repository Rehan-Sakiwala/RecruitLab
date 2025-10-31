using Server.Model.Entities;

namespace recruitlab.server.Model.Entities
{
    public class Application
    {
        public int Id { get; set; }

        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;

        public int JobOpeningId { get; set; }
        public JobOpening JobOpening { get; set; } = null!;

        public ApplicationStage Stage { get; set; } = ApplicationStage.Applied;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StatusUpdatedAt { get; set; }

        public int? AssignedReviewerId { get; set; }
        public User? AssignedReviewer { get; set; }
        public string? ScreeningNotes { get; set; }

        public string? RejectionReason { get; set; }

        public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }

    public enum ApplicationStage
    {
        Applied = 1,
        Screening = 2,
        Interview = 3,
        Offer = 4,
        Hired = 5,
        Rejected = 6,
        OnHold = 7
    }
}
