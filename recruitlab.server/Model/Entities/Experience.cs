namespace Server.Model.Entities
{
    public class Experience
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;
        public string Position { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;
        public string? Responsibilities { get; set; }
    }
}