using Server.Model.Entities;

namespace recruitlab.server.Model.Entities
{
    public class SkillRating
    {
        public int Id { get; set; }
        public int FeedbackId { get; set; }
        public Feedback Feedback { get; set; } = null!;

        public int SkillId { get; set; }
        public Skill Skill { get; set; } = null!;

        public int Rating { get; set; }
        public string? Comments { get; set; }
    }
}
