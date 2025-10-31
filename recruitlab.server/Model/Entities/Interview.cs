namespace recruitlab.server.Model.Entities
{
    public class Interview
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;

        public string RoundName { get; set; } = string.Empty;
        public RoundType RoundType { get; set; }
        public DateTime ScheduledTime { get; set; }
        public string? MeetLink { get; set; }
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;

        public ICollection<InterviewAssignment> Assignments { get; set; } = new List<InterviewAssignment>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }

    public enum RoundType
    {
        Technical = 1,
        HR = 2,
        Screening = 3
    }

    public enum InterviewStatus
    {
        Scheduled = 1,
        Completed = 2,
        Canceled = 3
    }
}
