using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using recruitlab.server.Data;
using recruitlab.server.Model.DTO.Server.Model.DTO;
using recruitlab.server.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace recruitlab.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InterviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InterviewController(AppDbContext context)
        {
            _context = context;
        }

        //Recruiter schedules Tech or HR round
        [HttpPost("schedule")]
        [Authorize(Roles = "Recruiter,Admin")]
        public async Task<IActionResult> ScheduleInterview([FromBody] ScheduleInterviewDto dto)
        {
            var app = await _context.Applications.FindAsync(dto.ApplicationId);
            if (app == null) return NotFound("Application not found.");

            var interview = new Interview
            {
                ApplicationId = dto.ApplicationId,
                RoundName = dto.RoundName,
                RoundType = dto.RoundType, // 1=Technical, 2=HR
                ScheduledTime = dto.ScheduledTime,
                MeetLink = dto.MeetLink,
                Status = InterviewStatus.Scheduled
            };

            foreach (var userId in dto.InterviewerIds)
            {
                interview.Assignments.Add(new InterviewAssignment { InterviewerId = userId });
            }

            _context.Interviews.Add(interview);

            if (app.Stage != ApplicationStage.Interview)
            {
                app.Stage = ApplicationStage.Interview;
                app.StatusUpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            //Remaining here: Mail Service

            return Ok(new { message = "Interview scheduled successfully.", interviewId = interview.Id });
        }

        //Get My Assigned Interviews
        [HttpGet("my-tasks")]
        [Authorize(Roles = "Interviewer,HR")]
        public async Task<IActionResult> GetMyInterviews()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var interviews = await _context.InterviewAssignments
                .Include(ia => ia.Interview).ThenInclude(i => i.Application).ThenInclude(a => a.Candidate).ThenInclude(c => c.User)
                .Include(ia => ia.Interview).ThenInclude(i => i.Application).ThenInclude(a => a.JobOpening)
                .Where(ia => ia.InterviewerId == userId && ia.Interview.Status == InterviewStatus.Scheduled)
                .Select(ia => new
                {
                    InterviewId = ia.Interview.Id,
                    Round = ia.Interview.RoundName,
                    Date = ia.Interview.ScheduledTime,
                    CandidateName = $"{ia.Interview.Application.Candidate.User.FirstName} {ia.Interview.Application.Candidate.User.LastName}",
                    JobTitle = ia.Interview.Application.JobOpening.Title,
                    Type = ia.Interview.RoundType.ToString()
                })
                .ToListAsync();

            return Ok(interviews);
        }

        //Submit Feedback
        [HttpPost("feedback")]
        [Authorize(Roles = "Interviewer,HR")]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var interview = await _context.Interviews.Include(i => i.Application).FirstOrDefaultAsync(i => i.Id == dto.InterviewId);

            if (interview == null) return NotFound("Interview not found.");

            var feedback = new Feedback
            {
                InterviewId = dto.InterviewId,
                InterviewerId = userId,
                OverallRating = dto.OverallRating,
                OverallComments = dto.Comments,
                SubmittedAt = DateTime.UtcNow
            };

            foreach (var skillDto in dto.SkillRatings)
            {
                feedback.SkillRatings.Add(new SkillRating
                {
                    SkillId = skillDto.SkillId,
                    Rating = skillDto.Rating,
                    Comments = skillDto.Comments
                });
            }

            _context.Feedbacks.Add(feedback);

            interview.Status = InterviewStatus.Completed;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Feedback submitted successfully." });
        }
    }
}
