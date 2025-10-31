using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using recruitlab.server.Data;
using recruitlab.server.Model.DTO;
using recruitlab.server.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace recruitlab.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Recruiter,HR,Interviewer")]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var screeningTasks = await _context.Applications
                .Include(a => a.Candidate.User)
                .Include(a => a.JobOpening)
                .Where(a => a.AssignedReviewerId == userId && a.Stage == ApplicationStage.Screening)
                .Select(a => new ScreeningTaskDto
                {
                    ApplicationId = a.Id,
                    CandidateName = $"{a.Candidate.User.FirstName} {a.Candidate.User.LastName}",
                    JobTitle = a.JobOpening.Title,
                    AppliedAt = a.AppliedAt
                })
                .ToListAsync();

            var interviewTasks = await _context.InterviewAssignments
                .Include(a => a.Interview.Application.Candidate.User)
                .Include(a => a.Interview.Application.JobOpening)
                .Where(a => a.InterviewerId == userId && a.Interview.Status == InterviewStatus.Scheduled)
                .Select(a => new InterviewTaskDto
                {
                    InterviewId = a.InterviewId,
                    ApplicationId = a.Interview.ApplicationId,
                    CandidateName = $"{a.Interview.Application.Candidate.User.FirstName} {a.Interview.Application.Candidate.User.LastName}",
                    JobTitle = a.Interview.Application.JobOpening.Title,
                    RoundName = a.Interview.RoundName,
                    ScheduledTime = a.Interview.ScheduledTime,
                    MeetLink = a.Interview.MeetLink
                })
                .ToListAsync();

            var tasks = new UserTaskDto
            {
                ScreeningTasks = screeningTasks,
                InterviewTasks = interviewTasks
            };

            return Ok(tasks);
        }
    }
}
