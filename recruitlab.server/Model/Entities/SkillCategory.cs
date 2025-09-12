namespace Server.Model.Entities
{
    public class SkillCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
