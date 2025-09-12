namespace Server.Model.DTO
{
    public class JobSkillDto
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string SkillCategoryName { get; set; }
        public int RequirementType { get; set; }
        public string RequirementTypeName { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateJobSkillDto
    {
        public int SkillId { get; set; }
        public int RequirementType { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateJobSkillDto
    {
        public int RequirementType { get; set; }
        public int? YearsOfExperience { get; set; }
        public string? Notes { get; set; }
    }
}
