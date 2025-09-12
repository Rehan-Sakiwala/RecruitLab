namespace Server.Model.Entities
{
    public enum SkillRequirementType
    {
        Required = 1,
        Preferred = 2
    }

    public class JobSkill
    {
        public int Id { get; set; }
        public int JobOpeningId { get; set; }
        public int SkillId { get; set; }
        public SkillRequirementType RequirementType { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
        
        public JobOpening JobOpening { get; set; }
        public Skill Skill { get; set; }
    }
}
