using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidateController : ControllerBase
    {
        private readonly IRepository<Candidate> candidateRepo;
        private readonly IRepository<CandidateSkill> candidateSkillRepo;
        private readonly IRepository<Skill> skillRepo;
        private readonly IRepository<User> userRepo;
        private readonly AppDbContext dbContext;

        public CandidateController(
            IRepository<Candidate> candidateRepo,
            IRepository<CandidateSkill> candidateSkillRepo,
            IRepository<Skill> skillRepo,
            IRepository<User> userRepo,
            AppDbContext dbContext)
        {
            this.candidateRepo = candidateRepo;
            this.candidateSkillRepo = candidateSkillRepo;
            this.skillRepo = skillRepo;
            this.userRepo = userRepo;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCandidates(
            [FromQuery] int? status = null,
            [FromQuery] int? source = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = dbContext.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .Include(c => c.CandidateCVs.Where(cv => cv.IsActive))
                .Include(c => c.CandidateJobMatches.Where(cjm => cjm.IsActive))
                .AsQueryable();

            // Apply filters
            if (status.HasValue)
                query = query.Where(c => (int)c.Status == status.Value);

            if (source.HasValue)
                query = query.Where(c => (int)c.Source == source.Value);

            if (isAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == isAvailable.Value);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => 
                    c.FirstName.Contains(search) ||
                    c.LastName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.CurrentPosition.Contains(search) ||
                    c.CurrentCompany.Contains(search));
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var candidates = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var candidateDtos = candidates.Select(MapToCandidateDto).ToList();

            return Ok(new
            {
                Candidates = candidateDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCandidate(int id)
        {
            var candidate = await dbContext.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .Include(c => c.CandidateCVs)
                .Include(c => c.CandidateJobMatches)
                    .ThenInclude(cjm => cjm.JobOpening)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            return Ok(MapToCandidateDto(candidate));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCandidate([FromBody] CreateCandidateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Check if candidate with same email already exists
            var existingCandidate = await dbContext.Candidates
    .FirstOrDefaultAsync(c => c.Email == dto.Email);
            if (existingCandidate != null)
                return BadRequest(new { message = "A candidate with this email already exists" });

            var candidate = new Candidate
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                DateOfBirth = dto.DateOfBirth,
                CurrentPosition = dto.CurrentPosition,
                CurrentCompany = dto.CurrentCompany,
                CurrentSalary = dto.CurrentSalary,
                ExpectedSalary = dto.ExpectedSalary,
                ExperienceSummary = dto.ExperienceSummary,
                Education = dto.Education,
                Certifications = dto.Certifications,
                Languages = dto.Languages,
                Notes = dto.Notes,
                LinkedInProfile = dto.LinkedInProfile,
                PortfolioUrl = dto.PortfolioUrl,
                Source = (CandidateSource)dto.Source,
                SourceDetails = dto.SourceDetails,
                IsAvailable = dto.IsAvailable,
                AvailableFromDate = dto.AvailableFromDate,
                CreatedByUserId = userId.Value,
                Status = CandidateStatus.Active
            };

            await candidateRepo.AddAsync(candidate);
            await candidateRepo.SaveChangesAsync();

            foreach (var skillDto in dto.CandidateSkills)
            {
                var candidateSkill = new CandidateSkill
                {
                    CandidateId = candidate.Id,
                    SkillId = skillDto.SkillId,
                    Level = (SkillLevel)skillDto.Level,
                    YearsOfExperience = skillDto.YearsOfExperience,
                    Notes = skillDto.Notes,
                    LastUsed = skillDto.LastUsed,
                    IsVerified = skillDto.IsVerified
                };
                await candidateSkillRepo.AddAsync(candidateSkill);
            }

            await candidateRepo.SaveChangesAsync();

            var createdCandidate = await dbContext.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .FirstOrDefaultAsync(c => c.Id == candidate.Id);

            return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, MapToCandidateDto(createdCandidate));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCandidate(int id, [FromBody] UpdateCandidateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var candidate = await candidateRepo.FindByIdAsync(id);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var userRole = GetCurrentUserRole();
            if (candidate.CreatedByUserId != userId.Value && userRole != "Admin")
                return Forbid();

            if (candidate.Email != dto.Email)
            {
                var existingCandidate = await dbContext.Candidates
    .FirstOrDefaultAsync(c => c.Email == dto.Email);
                if (existingCandidate != null)
                    return BadRequest(new { message = "A candidate with this email already exists" });
            }

            candidate.FirstName = dto.FirstName;
            candidate.LastName = dto.LastName;
            candidate.Email = dto.Email;
            candidate.PhoneNumber = dto.PhoneNumber;
            candidate.Address = dto.Address;
            candidate.City = dto.City;
            candidate.State = dto.State;
            candidate.Country = dto.Country;
            candidate.PostalCode = dto.PostalCode;
            candidate.DateOfBirth = dto.DateOfBirth;
            candidate.CurrentPosition = dto.CurrentPosition;
            candidate.CurrentCompany = dto.CurrentCompany;
            candidate.CurrentSalary = dto.CurrentSalary;
            candidate.ExpectedSalary = dto.ExpectedSalary;
            candidate.ExperienceSummary = dto.ExperienceSummary;
            candidate.Education = dto.Education;
            candidate.Certifications = dto.Certifications;
            candidate.Languages = dto.Languages;
            candidate.Notes = dto.Notes;
            candidate.LinkedInProfile = dto.LinkedInProfile;
            candidate.PortfolioUrl = dto.PortfolioUrl;
            candidate.Status = (CandidateStatus)dto.Status;
            candidate.Source = (CandidateSource)dto.Source;
            candidate.SourceDetails = dto.SourceDetails;
            candidate.LastContactDate = dto.LastContactDate;
            candidate.LastContactNotes = dto.LastContactNotes;
            candidate.IsAvailable = dto.IsAvailable;
            candidate.AvailableFromDate = dto.AvailableFromDate;
            candidate.UpdatedAt = DateTime.UtcNow;

            candidateRepo.Update(candidate);
            await candidateRepo.SaveChangesAsync();

            var updatedCandidate = await dbContext.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .FirstOrDefaultAsync(c => c.Id == id);

            return Ok(MapToCandidateDto(updatedCandidate));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            var candidate = await candidateRepo.FindByIdAsync(id);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var userRole = GetCurrentUserRole();
            if (candidate.CreatedByUserId != userId.Value && userRole != "Admin")
                return Forbid();

            await candidateRepo.DeleteAsync(id);
            await candidateRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/skills")]
        public async Task<IActionResult> AddSkillToCandidate(int id, [FromBody] CreateCandidateSkillDto dto)
        {
            var candidate = await candidateRepo.FindByIdAsync(id);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            var skill = await skillRepo.FindByIdAsync(dto.SkillId);
            if (skill == null)
                return NotFound(new { message = "Skill not found" });

            var existingCandidateSkill = await dbContext.CandidateSkills
                .FirstOrDefaultAsync(cs => cs.CandidateId == id && cs.SkillId == dto.SkillId);

            if (existingCandidateSkill != null)
                return BadRequest(new { message = "Skill already added to this candidate" });

            var candidateSkill = new CandidateSkill
            {
                CandidateId = id,
                SkillId = dto.SkillId,
                Level = (SkillLevel)dto.Level,
                YearsOfExperience = dto.YearsOfExperience,
                Notes = dto.Notes,
                LastUsed = dto.LastUsed,
                IsVerified = dto.IsVerified
            };

            await candidateSkillRepo.AddAsync(candidateSkill);
            await candidateSkillRepo.SaveChangesAsync();

            return Ok(new { message = "Skill added to candidate successfully" });
        }

        [HttpPut("{id}/skills/{skillId}")]
        public async Task<IActionResult> UpdateCandidateSkill(int id, int skillId, [FromBody] UpdateCandidateSkillDto dto)
        {
            var candidateSkill = await dbContext.CandidateSkills
                .FirstOrDefaultAsync(cs => cs.CandidateId == id && cs.SkillId == skillId);

            if (candidateSkill == null)
                return NotFound(new { message = "Candidate skill not found" });

            candidateSkill.Level = (SkillLevel)dto.Level;
            candidateSkill.YearsOfExperience = dto.YearsOfExperience;
            candidateSkill.Notes = dto.Notes;
            candidateSkill.LastUsed = dto.LastUsed;
            candidateSkill.IsVerified = dto.IsVerified;
            candidateSkill.UpdatedAt = DateTime.UtcNow;

            candidateSkillRepo.Update(candidateSkill);
            await candidateSkillRepo.SaveChangesAsync();

            return Ok(new { message = "Candidate skill updated successfully" });
        }

        [HttpDelete("{id}/skills/{skillId}")]
        public async Task<IActionResult> RemoveSkillFromCandidate(int id, int skillId)
        {
            var candidateSkill = await dbContext.CandidateSkills
                .FirstOrDefaultAsync(cs => cs.CandidateId == id && cs.SkillId == skillId);

            if (candidateSkill == null)
                return NotFound(new { message = "Candidate skill not found" });

            await candidateSkillRepo.DeleteAsync(candidateSkill.Id);
            await candidateSkillRepo.SaveChangesAsync();

            return Ok(new { message = "Skill removed from candidate successfully" });
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetCandidatesByStatus(int status)
        {
            var candidates = await dbContext.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .Where(c => (int)c.Status == status)
                .ToListAsync();

            var candidateDtos = candidates.Select(MapToCandidateDto).ToList();
            return Ok(candidateDtos);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCandidates([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest(new { message = "Search query is required" });

            var candidates = await dbContext.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .Where(c => 
                    c.FirstName.Contains(query) ||
                    c.LastName.Contains(query) ||
                    c.Email.Contains(query) ||
                    c.CurrentPosition.Contains(query) ||
                    c.CurrentCompany.Contains(query) ||
                    c.ExperienceSummary.Contains(query))
                .Take(50) // Limit results
                .ToListAsync();

            var candidateDtos = candidates.Select(MapToCandidateDto).ToList();
            return Ok(candidateDtos);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        }

        private CandidateDto MapToCandidateDto(Candidate candidate)
        {
            return new CandidateDto
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Email = candidate.Email,
                PhoneNumber = candidate.PhoneNumber,
                Address = candidate.Address,
                City = candidate.City,
                State = candidate.State,
                Country = candidate.Country,
                PostalCode = candidate.PostalCode,
                DateOfBirth = candidate.DateOfBirth,
                CurrentPosition = candidate.CurrentPosition,
                CurrentCompany = candidate.CurrentCompany,
                CurrentSalary = candidate.CurrentSalary,
                ExpectedSalary = candidate.ExpectedSalary,
                ExperienceSummary = candidate.ExperienceSummary,
                Education = candidate.Education,
                Certifications = candidate.Certifications,
                Languages = candidate.Languages,
                Notes = candidate.Notes,
                LinkedInProfile = candidate.LinkedInProfile,
                PortfolioUrl = candidate.PortfolioUrl,
                Status = (int)candidate.Status,
                StatusName = candidate.Status.ToString(),
                Source = (int)candidate.Source,
                SourceName = candidate.Source.ToString(),
                SourceDetails = candidate.SourceDetails,
                CreatedAt = candidate.CreatedAt,
                UpdatedAt = candidate.UpdatedAt,
                CreatedByUserId = candidate.CreatedByUserId,
                CreatedByUserEmail = candidate.CreatedByUser?.Email ?? "",
                LastContactDate = candidate.LastContactDate,
                LastContactNotes = candidate.LastContactNotes,
                IsAvailable = candidate.IsAvailable,
                AvailableFromDate = candidate.AvailableFromDate,
                CandidateSkills = candidate.CandidateSkills.Select(cs => new CandidateSkillDto
                {
                    Id = cs.Id,
                    CandidateId = cs.CandidateId,
                    SkillId = cs.SkillId,
                    SkillName = cs.Skill?.Name ?? "",
                    SkillCategoryName = cs.Skill?.SkillCategory?.Name ?? "",
                    Level = (int)cs.Level,
                    LevelName = cs.Level.ToString(),
                    YearsOfExperience = cs.YearsOfExperience,
                    Notes = cs.Notes,
                    LastUsed = cs.LastUsed,
                    IsVerified = cs.IsVerified,
                    CreatedAt = cs.CreatedAt,
                    UpdatedAt = cs.UpdatedAt
                }).ToList(),
                CandidateCVs = candidate.CandidateCVs.Select(cv => new CandidateCVDto
                {
                    Id = cv.Id,
                    CandidateId = cv.CandidateId,
                    FileName = cv.FileName,
                    FilePath = cv.FilePath,
                    FileType = cv.FileType,
                    FileSize = cv.FileSize,
                    Type = (int)cv.Type,
                    TypeName = cv.Type.ToString(),
                    Status = (int)cv.Status,
                    StatusName = cv.Status.ToString(),
                    ParsedContent = cv.ParsedContent,
                    ParsedSkills = cv.ParsedSkills,
                    ParsedExperience = cv.ParsedExperience,
                    ParsedEducation = cv.ParsedEducation,
                    ProcessingNotes = cv.ProcessingNotes,
                    UploadedAt = cv.UploadedAt,
                    ProcessedAt = cv.ProcessedAt,
                    UploadedByUserId = cv.UploadedByUserId,
                    UploadedByUserEmail = cv.UploadedByUser?.Email ?? "",
                    IsActive = cv.IsActive
                }).ToList(),
                CandidateJobMatches = candidate.CandidateJobMatches.Select(cjm => new CandidateJobMatchDto
                {
                    Id = cjm.Id,
                    CandidateId = cjm.CandidateId,
                    CandidateName = $"{candidate.FirstName} {candidate.LastName}",
                    CandidateEmail = candidate.Email,
                    JobOpeningId = cjm.JobOpeningId,
                    JobTitle = cjm.JobOpening?.Title ?? "",
                    JobDepartment = cjm.JobOpening?.Department ?? "",
                    Status = (int)cjm.Status,
                    StatusName = cjm.Status.ToString(),
                    Type = (int)cjm.Type,
                    TypeName = cjm.Type.ToString(),
                    MatchScore = cjm.MatchScore,
                    MatchDetails = cjm.MatchDetails,
                    Notes = cjm.Notes,
                    MatchedAt = cjm.MatchedAt,
                    StatusUpdatedAt = cjm.StatusUpdatedAt,
                    MatchedByUserId = cjm.MatchedByUserId,
                    MatchedByUserEmail = cjm.MatchedByUser?.Email ?? "",
                    InterviewScheduledAt = cjm.InterviewScheduledAt,
                    InterviewNotes = cjm.InterviewNotes,
                    OfferedSalary = cjm.OfferedSalary,
                    OfferDate = cjm.OfferDate,
                    ResponseDeadline = cjm.ResponseDeadline,
                    IsActive = cjm.IsActive
                }).ToList()
            };
        }
    }
}
