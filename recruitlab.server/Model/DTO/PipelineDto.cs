namespace recruitlab.server.Model.DTO
{
    using recruitlab.server.Model.Entities;
    using System.ComponentModel.DataAnnotations;

    namespace Server.Model.DTO
    {
        // 1. Screening
        public class ScreeningResultDto
        {
            [Required]
            public bool IsShortlisted { get; set; } // true = Move to Interview, false = Reject
            [Required]
            public string Comments { get; set; } = string.Empty;
        }

        // 2. Scheduling Interview
        public class ScheduleInterviewDto
        {
            [Required]
            public int ApplicationId { get; set; }
            [Required]
            public string RoundName { get; set; } = string.Empty;
            [Required]
            public RoundType RoundType { get; set; } // 1=Technical, 2=HR
            [Required]
            public DateTime ScheduledTime { get; set; }
            public string? MeetLink { get; set; }

            [Required]
            public List<int> InterviewerIds { get; set; } = new List<int>();
        }

        // 3. Submitting Feedback
        public class SubmitFeedbackDto
        {
            [Required]
            public int InterviewId { get; set; }
            [Required]
            public FeedbackRating OverallRating { get; set; }
            [Required]
            public string Comments { get; set; } = string.Empty;

            public List<SkillRatingDto> SkillRatings { get; set; } = new List<SkillRatingDto>();
        }

        public class SkillRatingDto
        {
            public int SkillId { get; set; }
            public int Rating { get; set; }
            public string? Comments { get; set; }
        }
    }
}
