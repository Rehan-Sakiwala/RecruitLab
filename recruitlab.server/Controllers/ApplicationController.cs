
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
    public class ApplicationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Application> _appRepo;
        private readonly IEmailService _emailService;

        public ApplicationController(AppDbContext context, IRepository<Application> appRepo, IEmailService emailService)
        {
            _context = context;
            _appRepo = appRepo;
            _emailService = emailService;
        }

        [HttpPost("assign")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> AssignCandidate([FromBody] CreateApplicationDto dto)
        {
            var exists = await _context.Applications.AnyAsync(a => a.CandidateId == dto.CandidateId && a.JobOpeningId == dto.JobOpeningId);
            if (exists)
                return BadRequest(new { message = "Candidate is already associated with this job." });

            var app = new Application
            {
                CandidateId = dto.CandidateId,
                JobOpeningId = dto.JobOpeningId,
                Stage = ApplicationStage.Applied
            };

            await _appRepo.AddAsync(app);
            await _appRepo.SaveChangesAsync();
            return Ok(app);
        }

        [HttpPost("apply")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> ApplyForJob([FromBody] CandidateApplyDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
            if (candidate == null)
                return NotFound(new { message = "Candidate profile not found." });

            var exists = await _context.Applications.AnyAsync(a => a.CandidateId == candidate.Id && a.JobOpeningId == dto.JobOpeningId);
            if (exists)
                return BadRequest(new { message = "You have already applied for this job." });

            var app = new Application
            {
                CandidateId = candidate.Id,
                JobOpeningId = dto.JobOpeningId,
                Stage = ApplicationStage.Applied
            };

            await _appRepo.AddAsync(app);
            await _appRepo.SaveChangesAsync();
            return Ok(app);
        }

        [HttpGet("job/{jobId}")]
        [Authorize(Roles = "Admin,Recruiter,HR")]
        public async Task<IActionResult> GetApplicationsForJob(int jobId)
        {
            var applications = await _context.Applications
                .Include(a => a.Candidate.User)
                .Include(a => a.JobOpening)
                .Include(a => a.AssignedReviewer)
                .Where(a => a.JobOpeningId == jobId)
                .Select(a => new ApplicationSummaryDto
                {
                    Id = a.Id,
                    CandidateId = a.CandidateId,
                    CandidateName = $"{a.Candidate.User.FirstName} {a.Candidate.User.LastName}",
                    JobOpeningId = a.JobOpeningId,
                    JobTitle = a.JobOpening.Title,
                    Stage = a.Stage,
                    AppliedAt = a.AppliedAt,
                    AssignedReviewerId = a.AssignedReviewerId,
                    AssignedReviewerName = a.AssignedReviewer != null ? $"{a.AssignedReviewer.FirstName} {a.AssignedReviewer.LastName}" : null
                })
                .ToListAsync();

            return Ok(applications);
        }

        [HttpGet("my-applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.UserId == userId);
            if (candidate == null)
                return Ok(new List<ApplicationSummaryDto>());

            var applications = await _context.Applications
                .Include(a => a.JobOpening)
                .Where(a => a.CandidateId == candidate.Id)
                .Select(a => new ApplicationSummaryDto
                {
                    Id = a.Id,
                    CandidateId = a.CandidateId,
                    CandidateName = $"{candidate.User.FirstName} {candidate.User.LastName}",
                    JobOpeningId = a.JobOpeningId,
                    JobTitle = a.JobOpening.Title,
                    Stage = a.Stage,
                    AppliedAt = a.AppliedAt
                })
                .ToListAsync();

            return Ok(applications);
        }

        [HttpPut("{id}/assign-reviewer")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> AssignReviewer(int id, [FromBody] AssignReviewerDto dto)
        {
            var app = await _context.Applications
                .Include(a => a.Candidate.User)
                .Include(a => a.JobOpening)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null) return NotFound();

            var reviewer = await _context.Users.FindAsync(dto.ReviewerId);
            if (reviewer == null) return BadRequest(new { message = "Reviewer not found." });

            app.AssignedReviewerId = dto.ReviewerId;
            app.Stage = ApplicationStage.Screening;
            app.StatusUpdatedAt = DateTime.UtcNow;

            _appRepo.Update(app);
            await _appRepo.SaveChangesAsync();

            string subject = $"Screening Request: {app.JobOpening.Title} for {app.Candidate.User.FirstName}";
            string body = $"You have been assigned to screen {app.Candidate.User.FirstName} for the role of {app.JobOpening.Title}.";
            await _emailService.SendEmailAsync(reviewer.Email, subject, body);

            return Ok(app);
        }

        [HttpPost("{id}/submit-screening")]
        [Authorize(Roles = "Admin,Recruiter,HR,Interviewer")]
        public async Task<IActionResult> SubmitScreening(int id, [FromBody] SubmitScreeningDto dto)
        {
            var app = await _appRepo.FindByIdAsync(id);
            if (app == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (app.AssignedReviewerId != userId)
                return Forbid();

            if (dto.NewStage != ApplicationStage.Interview && dto.NewStage != ApplicationStage.Rejected)
                return BadRequest(new { message = "Invalid target stage. Must be Interview or Rejected." });

            app.Stage = dto.NewStage;
            app.ScreeningNotes = dto.ScreeningNotes;
            app.RejectionReason = dto.NewStage == ApplicationStage.Rejected ? dto.ScreeningNotes : null;
            app.StatusUpdatedAt = DateTime.UtcNow;

            _appRepo.Update(app);
            await _appRepo.SaveChangesAsync();
            return Ok(app);
        }
    }
}