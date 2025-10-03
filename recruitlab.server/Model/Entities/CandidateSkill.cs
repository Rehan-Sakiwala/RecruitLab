namespace Server.Model.Entities
{
    public enum SkillLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }

    public class CandidateSkill
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int SkillId { get; set; }
        public SkillLevel Level { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsVerified { get; set; } = false; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Candidate Candidate { get; set; }
        public Skill Skill { get; set; }
    }
}
