using Server.Model.Entities;

namespace recruitlab.server.Model.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public Interview Interview { get; set; } = null!;

        public int InterviewerId { get; set; }
        public User Interviewer { get; set; } = null!;

        public FeedbackRating OverallRating { get; set; }
        public string OverallComments { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SkillRating> SkillRatings { get; set; } = new List<SkillRating>();
    }

    public enum FeedbackRating
    {
        StronglyNotRecommended = 1,
        NotRecommended = 2,
        Neutral = 3,
        Recommended = 4,
        StronglyRecommended = 5
    }
}
