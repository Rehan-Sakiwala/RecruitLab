using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class JobMatchingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<CandidateJobMatch> _candidateJobMatchRepository;
        private readonly ICandidateMatchingService _candidateMatchingService;

        public JobMatchingController(
            AppDbContext context,
            IRepository<CandidateJobMatch> candidateJobMatchRepository,
            ICandidateMatchingService candidateMatchingService)
        {
            _context = context;
            _candidateJobMatchRepository = candidateJobMatchRepository;
            _candidateMatchingService = candidateMatchingService;
        }

        [HttpGet("matches")]
        public async Task<IActionResult> GetAllMatches(
            [FromQuery] int? candidateId = null,
            [FromQuery] int? jobOpeningId = null,
            [FromQuery] int? status = null,
            [FromQuery] int? type = null,
            [FromQuery] decimal? minMatchScore = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.CandidateJobMatches
                .Include(cjm => cjm.Candidate)
                .Include(cjm => cjm.JobOpening)
                .Include(cjm => cjm.MatchedByUser)
                .Where(cjm => cjm.IsActive)
                .AsQueryable();

            // Apply filters
            if (candidateId.HasValue)
                query = query.Where(cjm => cjm.CandidateId == candidateId.Value);

            if (jobOpeningId.HasValue)
                query = query.Where(cjm => cjm.JobOpeningId == jobOpeningId.Value);

            if (status.HasValue)
                query = query.Where(cjm => (int)cjm.Status == status.Value);

            if (type.HasValue)
                query = query.Where(cjm => (int)cjm.Type == type.Value);

            if (minMatchScore.HasValue)
                query = query.Where(cjm => cjm.MatchScore >= minMatchScore.Value);

            // Apply pagination
            var totalCount = await query.CountAsync();
            var matches = await query
                .OrderByDescending(cjm => cjm.MatchScore)
                .ThenByDescending(cjm => cjm.MatchedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var matchDtos = matches.Select(MapToCandidateJobMatchDto).ToList();

            return Ok(new
            {
                Matches = matchDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("matches/{id}")]
        public async Task<IActionResult> GetMatch(int id)
        {
            var match = await _context.CandidateJobMatches
                .Include(cjm => cjm.Candidate)
                .Include(cjm => cjm.JobOpening)
                .Include(cjm => cjm.MatchedByUser)
                .FirstOrDefaultAsync(cjm => cjm.Id == id);

            if (match == null)
                return NotFound(new { message = "Match not found" });

            return Ok(MapToCandidateJobMatchDto(match));
        }

        [HttpPost("find-candidates/{jobOpeningId}")]
        public async Task<IActionResult> FindMatchingCandidates(int jobOpeningId, [FromBody] JobMatchRequestDto request)
        {
            if (jobOpeningId != request.JobOpeningId)
                return BadRequest(new { message = "Job opening ID mismatch" });

            var matches = await _candidateMatchingService.FindMatchingCandidatesAsync(request);
            return Ok(matches);
        }

        [HttpPost("create-match")]
        public async Task<IActionResult> CreateMatch([FromBody] CreateCandidateJobMatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Verify candidate exists
            var candidate = await _context.Candidates.FindAsync(dto.CandidateId);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            // Verify job opening exists
            var jobOpening = await _context.JobOpenings.FindAsync(dto.JobOpeningId);
            if (jobOpening == null)
                return NotFound(new { message = "Job opening not found" });

            var result = await _candidateMatchingService.CreateCandidateJobMatchAsync(
                dto.CandidateId, 
                dto.JobOpeningId, 
                userId.Value, 
                (MatchType)dto.Type);

            return Ok(result);
        }

        [HttpPut("matches/{id}/status")]
        public async Task<IActionResult> UpdateMatchStatus(int id, [FromBody] UpdateMatchStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var match = await _candidateJobMatchRepository.FindByIdAsync(id);
            if (match == null)
                return NotFound(new { message = "Match not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            match.Status = (MatchStatus)dto.Status;
            match.StatusUpdatedAt = DateTime.UtcNow;
            match.Notes = dto.Notes;

            // Add specific fields based on status
            switch ((MatchStatus)dto.Status)
            {
                case MatchStatus.InterviewScheduled:
                    match.InterviewScheduledAt = dto.InterviewScheduledAt;
                    break;
                case MatchStatus.Interviewed:
                    match.InterviewNotes = dto.InterviewNotes;
                    break;
                case MatchStatus.Offered:
                    match.OfferedSalary = dto.OfferedSalary;
                    match.OfferDate = DateTime.UtcNow;
                    match.ResponseDeadline = dto.ResponseDeadline;
                    break;
            }

            _candidateJobMatchRepository.Update(match);
            await _candidateJobMatchRepository.SaveChangesAsync();

            var updatedMatch = await _context.CandidateJobMatches
                .Include(cjm => cjm.Candidate)
                .Include(cjm => cjm.JobOpening)
                .Include(cjm => cjm.MatchedByUser)
                .FirstOrDefaultAsync(cjm => cjm.Id == id);

            return Ok(MapToCandidateJobMatchDto(updatedMatch));
        }

        [HttpDelete("matches/{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _candidateJobMatchRepository.FindByIdAsync(id);
            if (match == null)
                return NotFound(new { message = "Match not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Soft delete
            match.IsActive = false;
            _candidateJobMatchRepository.Update(match);
            await _candidateJobMatchRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("candidates/{candidateId}/matches")]
        public async Task<IActionResult> GetCandidateMatches(int candidateId)
        {
            var matches = await _context.CandidateJobMatches
                .Include(cjm => cjm.Candidate)
                .Include(cjm => cjm.JobOpening)
                .Include(cjm => cjm.MatchedByUser)
                .Where(cjm => cjm.CandidateId == candidateId && cjm.IsActive)
                .OrderByDescending(cjm => cjm.MatchedAt)
                .ToListAsync();

            var matchDtos = matches.Select(MapToCandidateJobMatchDto).ToList();
            return Ok(matchDtos);
        }

        [HttpGet("jobs/{jobOpeningId}/matches")]
        public async Task<IActionResult> GetJobMatches(int jobOpeningId)
        {
            var matches = await _context.CandidateJobMatches
                .Include(cjm => cjm.Candidate)
                .Include(cjm => cjm.JobOpening)
                .Include(cjm => cjm.MatchedByUser)
                .Where(cjm => cjm.JobOpeningId == jobOpeningId && cjm.IsActive)
                .OrderByDescending(cjm => cjm.MatchScore)
                .ToListAsync();

            var matchDtos = matches.Select(MapToCandidateJobMatchDto).ToList();
            return Ok(matchDtos);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetMatchingStatistics()
        {
            var stats = await _context.CandidateJobMatches
                .Where(cjm => cjm.IsActive)
                .GroupBy(cjm => cjm.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    StatusId = (int)g.Key,
                    Count = g.Count(),
                    AverageScore = g.Average(cjm => cjm.MatchScore)
                })
                .ToListAsync();

            var totalMatches = await _context.CandidateJobMatches.CountAsync(cjm => cjm.IsActive);
            var averageMatchScore = await _context.CandidateJobMatches
                .Where(cjm => cjm.IsActive)
                .AverageAsync(cjm => cjm.MatchScore);

            return Ok(new
            {
                TotalMatches = totalMatches,
                AverageMatchScore = averageMatchScore,
                StatusBreakdown = stats
            });
        }

        [HttpPost("auto-match/{jobOpeningId}")]
        public async Task<IActionResult> AutoMatchCandidatesForJob(int jobOpeningId, [FromBody] AutoMatchRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var jobOpening = await _context.JobOpenings
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == jobOpeningId);

            if (jobOpening == null)
                return NotFound(new { message = "Job opening not found" });

            // Find matching candidates
            var matchRequest = new JobMatchRequestDto
            {
                JobOpeningId = jobOpeningId,
                MinMatchScore = request.MinMatchScore,
                IncludeInactiveCandidates = request.IncludeInactiveCandidates,
                Limit = request.MaxMatches
            };

            var matches = await _candidateMatchingService.FindMatchingCandidatesAsync(matchRequest);
            var createdMatches = new List<CandidateJobMatchDto>();

            foreach (var match in matches.Take(request.MaxMatches))
            {
                var result = await _candidateMatchingService.CreateCandidateJobMatchAsync(
                    match.CandidateId,
                    jobOpeningId,
                    userId.Value,
                    MatchType.Automatic);

                createdMatches.Add(result);
            }

            return Ok(new
            {
                Message = $"Created {createdMatches.Count} automatic matches",
                Matches = createdMatches
            });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        private CandidateJobMatchDto MapToCandidateJobMatchDto(CandidateJobMatch match)
        {
            return new CandidateJobMatchDto
            {
                Id = match.Id,
                CandidateId = match.CandidateId,
                CandidateName = $"{match.Candidate.FirstName} {match.Candidate.LastName}",
                CandidateEmail = match.Candidate.Email,
                JobOpeningId = match.JobOpeningId,
                JobTitle = match.JobOpening.Title,
                JobDepartment = match.JobOpening.Department,
                Status = (int)match.Status,
                StatusName = match.Status.ToString(),
                Type = (int)match.Type,
                TypeName = match.Type.ToString(),
                MatchScore = match.MatchScore,
                MatchDetails = match.MatchDetails,
                Notes = match.Notes,
                MatchedAt = match.MatchedAt,
                StatusUpdatedAt = match.StatusUpdatedAt,
                MatchedByUserId = match.MatchedByUserId,
                MatchedByUserEmail = match.MatchedByUser?.Email ?? "",
                InterviewScheduledAt = match.InterviewScheduledAt,
                InterviewNotes = match.InterviewNotes,
                OfferedSalary = match.OfferedSalary,
                OfferDate = match.OfferDate,
                ResponseDeadline = match.ResponseDeadline,
                IsActive = match.IsActive
            };
        }
    }

    // Additional DTOs for job matching
    public class CreateCandidateJobMatchDto
    {
        public int CandidateId { get; set; }
        public int JobOpeningId { get; set; }
        public int Type { get; set; } = 2; // Manual by default
        public string? Notes { get; set; }
    }

    public class UpdateMatchStatusDto
    {
        public int Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? InterviewScheduledAt { get; set; }
        public string? InterviewNotes { get; set; }
        public decimal? OfferedSalary { get; set; }
        public DateTime? ResponseDeadline { get; set; }
    }

    public class AutoMatchRequestDto
    {
        public decimal MinMatchScore { get; set; } = 0.5m;
        public bool IncludeInactiveCandidates { get; set; } = false;
        public int MaxMatches { get; set; } = 10;
    }
}
