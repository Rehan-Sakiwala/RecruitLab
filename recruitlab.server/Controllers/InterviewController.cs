using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using recruitlab.server.Data;
using recruitlab.server.Model.DTO;
using recruitlab.server.Model.Entities;
using recruitlab.server.Services.Interface;
using Server.Data;
using Microsoft.EntityFrameworkCore;

namespace recruitlab.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InterviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Interview> _interviewRepo;
        private readonly IRepository<Feedback> _feedbackRepo;
        private readonly IEmailService _emailService;

        public InterviewController(
            AppDbContext context,
            IRepository<Interview> interviewRepo,
            IRepository<Feedback> feedbackRepo,
            IEmailService emailService)
        {
            _context = context;
            _interviewRepo = interviewRepo;
            _feedbackRepo = feedbackRepo;
            _emailService = emailService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> ScheduleInterview([FromBody] CreateInterviewDto dto)
        {
            var app = await _context.Applications
                .Include(a => a.Candidate.User)
                .Include(a => a.JobOpening)
                .FirstOrDefaultAsync(a => a.Id == dto.ApplicationId);

            if (app == null)
                return NotFound(new { message = "Application not found." });

            var interview = new Interview
            {
                ApplicationId = dto.ApplicationId,
                RoundName = dto.RoundName,
                RoundType = dto.RoundType,
                ScheduledTime = dto.ScheduledTime,
                MeetLink = dto.MeetLink,
                Status = InterviewStatus.Scheduled
            };

            foreach (var interviewerId in dto.InterviewerIds)
            {
                interview.Assignments.Add(new InterviewAssignment { InterviewerId = interviewerId });
            }

            await _interviewRepo.AddAsync(interview);

            app.Stage = ApplicationStage.Interview;
            await _interviewRepo.SaveChangesAsync();

            var interviewers = await _context.Users
                .Where(u => dto.InterviewerIds.Contains(u.Id))
                .ToListAsync();

            string subject = $"Interview Scheduled: {app.JobOpening.Title} with {app.Candidate.User.FirstName}";
            string body = $"An interview for {dto.RoundName} is scheduled at {dto.ScheduledTime}.<br>Meeting Link: {dto.MeetLink}";

            await _emailService.SendEmailAsync(app.Candidate.User.Email, subject, body);
            foreach (var interviewer in interviewers)
            {
                await _emailService.SendEmailAsync(interviewer.Email, subject, body);
            }

            return Ok(interview);
        }

        [HttpGet("application/{appId}")]
        [Authorize(Roles = "Admin,Recruiter,HR,Interviewer,Candidate")]
        public async Task<IActionResult> GetInterviewsForApplication(int appId)
        {
            var interviews = await _context.Interviews
                .Include(i => i.Assignments)
                .ThenInclude(a => a.Interviewer)
                .Include(i => i.Feedbacks)
                .Where(i => i.ApplicationId == appId)
                .ToListAsync();

            return Ok(interviews);
        }

        [HttpPost("feedback")]
        [Authorize(Roles = "Admin,Recruiter,HR,Interviewer")]
        public async Task<IActionResult> SubmitFeedback([FromBody] CreateFeedbackDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isAssigned = await _context.InterviewAssignments
                .AnyAsync(a => a.InterviewId == dto.InterviewId && a.InterviewerId == userId);

            if (!isAssigned)
                return Forbid();

            var feedback = new Feedback
            {
                InterviewId = dto.InterviewId,
                InterviewerId = userId,
                OverallRating = dto.OverallRating,
                OverallComments = dto.OverallComments
            };

            foreach (var sr in dto.SkillRatings)
            {
                feedback.SkillRatings.Add(new SkillRating
                {
                    SkillId = sr.SkillId,
                    Rating = sr.Rating,
                    Comments = sr.Comments
                });
            }

            await _feedbackRepo.AddAsync(feedback);
            await _feedbackRepo.SaveChangesAsync();

            var interview = await _interviewRepo.FindByIdAsync(dto.InterviewId);
            if (interview != null)
            {
                interview.Status = InterviewStatus.Completed;
                _interviewRepo.Update(interview);
                await _interviewRepo.SaveChangesAsync();
            }

            return Ok(feedback);
        }

        [HttpGet("feedback/{interviewId}")]
        [Authorize(Roles = "Admin,Recruiter,HR,Interviewer")]
        public async Task<IActionResult> GetFeedbackForInterview(int interviewId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Interviewer)
                .Include(f => f.SkillRatings)
                .ThenInclude(sr => sr.Skill)
                .Where(f => f.InterviewId == interviewId)
                .ToListAsync();

            return Ok(feedbacks);
        }
    }
}