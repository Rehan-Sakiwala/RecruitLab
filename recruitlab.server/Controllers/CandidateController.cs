using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;
using recruitlab.server.Model.DTO;
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
        private readonly IRepository<Candidate> _candidateRepo;
        private readonly IRepository<CandidateSkill> _candidateSkillRepo;
        private readonly IRepository<Skill> _skillRepo;
        private readonly AppDbContext _context;

        public CandidateController(
            IRepository<Candidate> candidateRepo,
            IRepository<CandidateSkill> candidateSkillRepo,
            IRepository<Skill> skillRepo,
            AppDbContext dbContext)
        {
            _candidateRepo = candidateRepo;
            _candidateSkillRepo = candidateSkillRepo;
            _skillRepo = skillRepo;
            _context = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCandidates(
            [FromQuery] int? source = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CreatedByUser.Role)
                .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill)
                .Include(c => c.ExperienceHistory)
                .AsQueryable();

            if (source.HasValue)
                query = query.Where(c => (int)c.Source == source.Value);

            if (isAvailable.HasValue)
                query = query.Where(c => c.IsAvailable == isAvailable.Value);

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                query = query.Where(c =>
                    c.User.FirstName.ToLower().Contains(s) ||
                    c.User.LastName.ToLower().Contains(s) ||
                    c.User.Email.ToLower().Contains(s) ||
                    c.ExperienceHistory.Any(e => e.Position.ToLower().Contains(s) || e.CompanyName.ToLower().Contains(s))
                );
            }

            var totalCount = await query.CountAsync();
            var candidates = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var candidateDtos = candidates.Select(MapToCandidateListDto).ToList();

            return Ok(new
            {
                Candidates = candidateDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Recruiter,HR,Interviewer")]
        public async Task<IActionResult> GetCandidate(int id)
        {
            var candidate = await _context.Candidates
                .Include(c => c.User)
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills).ThenInclude(cs => cs.Skill).ThenInclude(s => s.SkillCategory)
                .Include(c => c.EducationHistory)
                .Include(c => c.ExperienceHistory)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            return Ok(MapToCandidateProfileDto(candidate));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> DeleteCandidate(int id)
        {
            var candidate = await _candidateRepo.FindByIdAsync(id);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            var user = await _context.Users.FindAsync(candidate.UserId);
            if (user == null)
                return NotFound(new { message = "Associated user not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/skills")]
        [Authorize(Roles = "Admin,Recruiter")]
        public async Task<IActionResult> AddSkillToCandidate(int id, [FromBody] CreateCandidateSkillDto dto)
        {
            if (id != dto.CandidateId)
                return BadRequest("Mismatched candidate ID");

            var candidate = await _candidateRepo.FindByIdAsync(id);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            var skill = await _skillRepo.FindByIdAsync(dto.SkillId);
            if (skill == null)
                return NotFound(new { message = "Skill not found" });

            var existingSkill = await _context.CandidateSkills
                .FirstOrDefaultAsync(cs => cs.CandidateId == id && cs.SkillId == dto.SkillId);

            if (existingSkill != null)
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

            await _candidateSkillRepo.AddAsync(candidateSkill);
            await _candidateSkillRepo.SaveChangesAsync();

            return Ok(candidateSkill);
        }

        // Add other endpoints for Education, Experience, etc. here

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        // --- MAPPERS ---

        private CandidateListDto MapToCandidateListDto(Candidate c)
        {
            var currentExp = c.ExperienceHistory.FirstOrDefault(e => e.IsCurrent);
            return new CandidateListDto
            {
                Id = c.Id,
                UserId = c.UserId,
                FirstName = c.User.FirstName,
                LastName = c.User.LastName,
                Email = c.User.Email,
                PhoneNumber = c.User.PhoneNumber,
                CurrentPosition = currentExp?.Position,
                CurrentCompany = currentExp?.CompanyName,
                IsAvailable = c.IsAvailable,
                Source = c.Source.ToString(),
                CreatedAt = c.CreatedAt,
                Skills = c.CandidateSkills.Select(cs => cs.Skill.Name).ToList()
            };
        }

        private CandidateProfileDto MapToCandidateProfileDto(Candidate c)
        {
            return new CandidateProfileDto
            {
                Id = c.Id,
                UserId = c.UserId,
                FirstName = c.User.FirstName,
                LastName = c.User.LastName,
                Email = c.User.Email,
                PhoneNumber = c.User.PhoneNumber,
                LinkedInProfile = c.LinkedInProfile,
                PortfolioUrl = c.PortfolioUrl,
                CurrentSalary = c.CurrentSalary,
                ExpectedSalary = c.ExpectedSalary,
                Certifications = c.Certifications,
                Languages = c.Languages,
                Notes = c.Notes,
                Source = c.Source.ToString(),
                SourceDetails = c.SourceDetails,
                IsAvailable = c.IsAvailable,
                AvailableFromDate = c.AvailableFromDate,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedByUser.Email,
                CandidateSkills = c.CandidateSkills.Select(cs => new CandidateSkillDto
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
                    IsVerified = cs.IsVerified
                }).ToList(),
                EducationHistory = c.EducationHistory.Select(e => new EducationDto
                {
                    Id = e.Id,
                    SchoolName = e.SchoolName,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.IsCurrent
                }).ToList(),
                ExperienceHistory = c.ExperienceHistory.Select(e => new ExperienceDto
                {
                    Id = e.Id,
                    Position = e.Position,
                    CompanyName = e.CompanyName,
                    Location = e.Location,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    IsCurrent = e.IsCurrent,
                    Responsibilities = e.Responsibilities
                }).ToList(),
                Documents = c.Documents.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    FileType = d.FileType,
                    Type = d.Type.ToString(),
                    UploadedAt = d.UploadedAt
                }).ToList()
            };
        }
    }
}
