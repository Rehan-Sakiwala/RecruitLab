namespace Server.Model.DTO
{
    public class SkillCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSkillCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateSkillCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
