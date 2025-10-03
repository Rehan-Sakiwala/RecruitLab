namespace Server.Model.DTO
{
    public class CandidateSkillDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string SkillCategoryName { get; set; }
        public int Level { get; set; }
        public string LevelName { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCandidateSkillDto
    {
        public int SkillId { get; set; }
        public int Level { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = false;
    }

    public class UpdateCandidateSkillDto
    {
        public int Level { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsVerified { get; set; }
    }
}
