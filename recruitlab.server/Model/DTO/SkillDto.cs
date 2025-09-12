namespace Server.Model.DTO
{
    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SkillCategoryId { get; set; }
        public string SkillCategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSkillDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SkillCategoryId { get; set; }
    }

    public class UpdateSkillDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int SkillCategoryId { get; set; }
    }
}
