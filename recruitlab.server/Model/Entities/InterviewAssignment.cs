using Server.Model.Entities;

namespace recruitlab.server.Model.Entities
{
    public class InterviewAssignment
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public Interview Interview { get; set; } = null!;

        public int InterviewerId { get; set; }
        public User Interviewer { get; set; } = null!;
    }
}
