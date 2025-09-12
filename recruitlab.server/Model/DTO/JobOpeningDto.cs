namespace Server.Model.DTO
{
    public class JobOpeningDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string? StatusReason { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<JobSkillDto> JobSkills { get; set; } = new List<JobSkillDto>();
    }

    public class CreateJobOpeningDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public List<CreateJobSkillDto> JobSkills { get; set; } = new List<CreateJobSkillDto>();
    }

    public class UpdateJobOpeningDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public int Status { get; set; }
        public string? StatusReason { get; set; }
    }
}
