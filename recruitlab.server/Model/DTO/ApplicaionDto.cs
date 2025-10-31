
using recruitlab.server.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace recruitlab.server.Model.DTO
{
    public class CreateApplicationDto
    {
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public int JobOpeningId { get; set; }
    }

    public class CandidateApplyDto
    {
        [Required]
        public int JobOpeningId { get; set; }
    }

    public class AssignReviewerDto
    {
        [Required]
        public int ReviewerId { get; set; }
    }

    public class SubmitScreeningDto
    {
        [Required]
        public ApplicationStage NewStage { get; set; }
        public string? ScreeningNotes { get; set; }
    }

    public class CreateInterviewDto
    {
        [Required]
        public int ApplicationId { get; set; }
        [Required]
        public string RoundName { get; set; } = string.Empty;
        [Required]
        public RoundType RoundType { get; set; }
        [Required]
        public DateTime ScheduledTime { get; set; }
        public string? MeetLink { get; set; }

        [Required]
        public List<int> InterviewerIds { get; set; } = new List<int>();
    }

    public class CreateFeedbackDto
    {
        [Required]
        public int InterviewId { get; set; }
        [Required]
        public FeedbackRating OverallRating { get; set; }
        [Required]
        public string OverallComments { get; set; } = string.Empty;

        public List<CreateSkillRatingDto> SkillRatings { get; set; } = new List<CreateSkillRatingDto>();
    }

    public class CreateSkillRatingDto
    {
        [Required]
        public int SkillId { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comments { get; set; }
    }

    public class ApplicationSummaryDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int JobOpeningId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public ApplicationStage Stage { get; set; }
        public DateTime AppliedAt { get; set; }
        public int? AssignedReviewerId { get; set; }
        public string? AssignedReviewerName { get; set; }
    }

    public class UserTaskDto
    {
        public List<ScreeningTaskDto> ScreeningTasks { get; set; } = new List<ScreeningTaskDto>();
        public List<InterviewTaskDto> InterviewTasks { get; set; } = new List<InterviewTaskDto>();
    }

    public class ScreeningTaskDto
    {
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
    }

    public class InterviewTaskDto
    {
        public int InterviewId { get; set; }
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string RoundName { get; set; } = string.Empty;
        public DateTime ScheduledTime { get; set; }
        public string? MeetLink { get; set; }
    }
}
