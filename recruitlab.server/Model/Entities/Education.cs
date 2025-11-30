using Server.Model.Entities;

namespace recruitlab.server.Model.Entities
{
    namespace Server.Model.Entities
    {
        public class Education
        {
            public int Id { get; set; }
            public int CandidateId { get; set; }
            public Candidate Candidate { get; set; } = null!;
            public string SchoolName { get; set; } = string.Empty;
            public string Degree { get; set; } = string.Empty;
            public string FieldOfStudy { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public bool IsCurrent { get; set; } = false;
            public string? Notes { get; set; }
        }
    }
}
