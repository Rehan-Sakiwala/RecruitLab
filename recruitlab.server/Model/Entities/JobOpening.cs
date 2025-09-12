namespace Server.Model.Entities
{
    public enum JobStatus
    {
        Open = 1,
        OnHold = 2,
        Closed = 3
    }

    public class JobOpening
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Open;
        public string? StatusReason { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public User CreatedByUser { get; set; }
        public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }
}
