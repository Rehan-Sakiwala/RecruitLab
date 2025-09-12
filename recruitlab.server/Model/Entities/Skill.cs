namespace Server.Model.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SkillCategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public SkillCategory SkillCategory { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
