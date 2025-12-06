using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;
using recruitlab.server.Model.DTO;
using recruitlab.server.Model.Entities;
using Server.Data;
using Server.Model.Entities;

namespace recruitlab.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Application> _appRepo;

        public ApplicationController(AppDbContext context, IRepository<Application> appRepo)
        {
            _context = context;
            _appRepo = appRepo;
        }

        [HttpPost("apply")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> ApplyForJob([FromBody] CandidateApplyDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
            {
                return BadRequest(new { message = "Candidate profile not found. Please complete your profile first." });
            }

            var job = await _context.JobOpenings.FindAsync(dto.JobOpeningId);
            if (job == null)
                return NotFound(new { message = "Job opening not found." });

            if (job.Status != JobStatus.Open)
                return BadRequest(new { message = "This job is no longer accepting applications." });

            var existingApplication = await _context.Applications
                .AnyAsync(a => a.CandidateId == candidate.Id && a.JobOpeningId == dto.JobOpeningId);

            if (existingApplication)
            {
                return BadRequest(new { message = "You have already applied for this position." });
            }

            var application = new Application
            {
                CandidateId = candidate.Id,
                JobOpeningId = dto.JobOpeningId,
                Stage = ApplicationStage.Applied,
                AppliedAt = DateTime.UtcNow
            };

            await _appRepo.AddAsync(application);
            await _appRepo.SaveChangesAsync();

            return Ok(new { message = "Application submitted successfully.", applicationId = application.Id });
        }

        [HttpGet("my-applications")]
        [Authorize(Roles = "Candidate")]
        public async Task<IActionResult> GetMyApplications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var candidate = await _context.Candidates
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (candidate == null)
                return Ok(new List<ApplicationSummaryDto>());

            var applications = await _context.Applications
                .Include(a => a.JobOpening)
                .Where(a => a.CandidateId == candidate.Id)
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new ApplicationSummaryDto
                {
                    Id = a.Id,
                    JobOpeningId = a.JobOpeningId,
                    JobTitle = a.JobOpening.Title,
                    Department = a.JobOpening.Department,
                    Location = a.JobOpening.Location,
                    Stage = a.Stage.ToString(),
                    AppliedAt = a.AppliedAt
                })
                .ToListAsync();

            return Ok(applications);
        }
    }
}
