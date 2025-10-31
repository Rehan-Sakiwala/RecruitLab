using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;
using recruitlab.server.Model.DTO;
using recruitlab.server.Model.Entities.Server.Model.Entities;
using recruitlab.server.Services.Interface;
using Server.Data;
using Server.Model.Entities;

namespace recruitlab.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Education> _educationRepo;
        private readonly IRepository<Experience> _experienceRepo;
        private readonly IRepository<Document> _documentRepo;
        private readonly IRepository<CandidateSkill> _candidateSkillRepo;
        private readonly IFileService _fileService;

        public CandidateProfileController(
            AppDbContext context,
            IRepository<Education> educationRepo,
            IRepository<Experience> experienceRepo,
            IRepository<Document> documentRepo,
            IRepository<CandidateSkill> candidateSkillRepo,
            IFileService fileService)
        {
            _context = context;
            _educationRepo = educationRepo;
            _experienceRepo = experienceRepo;
            _documentRepo = documentRepo;
            _candidateSkillRepo = candidateSkillRepo;
            _fileService = fileService;
        }

        [HttpPost("education")]
        public async Task<IActionResult> AddEducation([FromBody] CreateEducationDto dto)
        {
            var candidate = await GetCandidateForCurrentUser();
            if (candidate == null)
                return NotFound(new { message = "Candidate profile not found." });

            var education = new Education
            {
                CandidateId = candidate.Id,
                SchoolName = dto.SchoolName,
                Degree = dto.Degree,
                FieldOfStudy = dto.FieldOfStudy,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCurrent = dto.IsCurrent,
                Notes = dto.Notes
            };

            await _educationRepo.AddAsync(education);
            await _educationRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEducation), new { id = education.Id }, education);
        }

        [HttpGet("education/{id}")]
        public async Task<IActionResult> GetEducation(int id)
        {
            var education = await _educationRepo.FindByIdAsync(id);
            if (education == null) return NotFound();
            if (!await CanAccessProfile(education.CandidateId)) return Forbid();
            return Ok(education);
        }

        [HttpPut("education/{id}")]
        public async Task<IActionResult> UpdateEducation(int id, [FromBody] UpdateEducationDto dto)
        {
            var education = await _educationRepo.FindByIdAsync(id);
            if (education == null) return NotFound();
            if (!await CanAccessProfile(education.CandidateId)) return Forbid();

            education.SchoolName = dto.SchoolName;
            education.Degree = dto.Degree;
            education.FieldOfStudy = dto.FieldOfStudy;
            education.StartDate = dto.StartDate;
            education.EndDate = dto.EndDate;
            education.IsCurrent = dto.IsCurrent;
            education.Notes = dto.Notes;

            _educationRepo.Update(education);
            await _educationRepo.SaveChangesAsync();
            return Ok(education);
        }

        [HttpDelete("education/{id}")]
        public async Task<IActionResult> DeleteEducation(int id)
        {
            var education = await _educationRepo.FindByIdAsync(id);
            if (education == null) return NotFound();
            if (!await CanAccessProfile(education.CandidateId)) return Forbid();

            await _educationRepo.DeleteAsync(id);
            await _educationRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("experience")]
        public async Task<IActionResult> AddExperience([FromBody] CreateExperienceDto dto)
        {
            var candidate = await GetCandidateForCurrentUser();
            if (candidate == null)
                return NotFound(new { message = "Candidate profile not found." });

            var experience = new Experience
            {
                CandidateId = candidate.Id,
                Position = dto.Position,
                CompanyName = dto.CompanyName,
                Location = dto.Location,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsCurrent = dto.IsCurrent,
                Responsibilities = dto.Responsibilities
            };

            await _experienceRepo.AddAsync(experience);
            await _experienceRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetExperience), new { id = experience.Id }, experience);
        }

        [HttpGet("experience/{id}")]
        public async Task<IActionResult> GetExperience(int id)
        {
            var experience = await _experienceRepo.FindByIdAsync(id);
            if (experience == null) return NotFound();
            if (!await CanAccessProfile(experience.CandidateId)) return Forbid();
            return Ok(experience);
        }

        [HttpPut("experience/{id}")]
        public async Task<IActionResult> UpdateExperience(int id, [FromBody] UpdateExperienceDto dto)
        {
            var experience = await _experienceRepo.FindByIdAsync(id);
            if (experience == null) return NotFound();
            if (!await CanAccessProfile(experience.CandidateId)) return Forbid();

            experience.Position = dto.Position;
            experience.CompanyName = dto.CompanyName;
            experience.Location = dto.Location;
            experience.StartDate = dto.StartDate;
            experience.EndDate = dto.EndDate;
            experience.IsCurrent = dto.IsCurrent;
            experience.Responsibilities = dto.Responsibilities;

            _experienceRepo.Update(experience);
            await _experienceRepo.SaveChangesAsync();
            return Ok(experience);
        }

        [HttpDelete("experience/{id}")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var experience = await _experienceRepo.FindByIdAsync(id);
            if (experience == null) return NotFound();
            if (!await CanAccessProfile(experience.CandidateId)) return Forbid();

            await _experienceRepo.DeleteAsync(id);
            await _experienceRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("skill")]
        public async Task<IActionResult> AddCandidateSkill([FromBody] CreateCandidateSkillDto dto)
        {
            var candidate = await GetCandidateForCurrentUser();
            if (candidate == null)
                return NotFound(new { message = "Candidate profile not found." });

            var exists = await _context.CandidateSkills.AnyAsync(s => s.CandidateId == candidate.Id && s.SkillId == dto.SkillId);
            if (exists)
                return BadRequest(new { message = "Skill already exists for this candidate." });

            var skill = new CandidateSkill
            {
                CandidateId = candidate.Id,
                SkillId = dto.SkillId,
                Level = (SkillLevel)dto.Level,
                YearsOfExperience = dto.YearsOfExperience,
                Notes = dto.Notes,
                LastUsed = dto.LastUsed,
                IsVerified = dto.IsVerified
            };

            await _candidateSkillRepo.AddAsync(skill);
            await _candidateSkillRepo.SaveChangesAsync();
            return Ok(skill);
        }

        [HttpPut("skill/{id}")]
        public async Task<IActionResult> UpdateCandidateSkill(int id, [FromBody] UpdateCandidateSkillDto dto)
        {
            var skill = await _candidateSkillRepo.FindByIdAsync(id);
            if (skill == null) return NotFound();
            if (!await CanAccessProfile(skill.CandidateId)) return Forbid();

            skill.Level = (SkillLevel)dto.Level;
            skill.YearsOfExperience = dto.YearsOfExperience;
            skill.Notes = dto.Notes;
            skill.LastUsed = dto.LastUsed;
            skill.IsVerified = dto.IsVerified;
            skill.UpdatedAt = DateTime.UtcNow;

            _candidateSkillRepo.Update(skill);
            await _candidateSkillRepo.SaveChangesAsync();
            return Ok(skill);
        }

        [HttpDelete("skill/{id}")]
        public async Task<IActionResult> DeleteCandidateSkill(int id)
        {
            var skill = await _candidateSkillRepo.FindByIdAsync(id);
            if (skill == null) return NotFound();
            if (!await CanAccessProfile(skill.CandidateId)) return Forbid();

            await _candidateSkillRepo.DeleteAsync(id);
            await _candidateSkillRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("cv")]
        public async Task<IActionResult> UploadCv([FromForm] UploadCvDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest(new { message = "File is empty." });

            var candidate = await GetCandidateForCurrentUser();
            if (candidate == null)
                return NotFound(new { message = "Candidate profile not found." });

            var filePath = await _fileService.UploadFileAsync(dto.File, "cvs");

            var document = new Document
            {
                CandidateId = candidate.Id,
                FileName = dto.File.FileName,
                FilePath = filePath,
                FileType = dto.File.ContentType,
                Type = DocumentType.CV
            };

            await _documentRepo.AddAsync(document);
            await _documentRepo.SaveChangesAsync();

            return Ok(new DocumentDto
            {
                Id = document.Id,
                FileName = document.FileName,
                FilePath = document.FilePath,
                FileType = document.FileType,
                Type = document.Type.ToString(),
                UploadedAt = document.UploadedAt
            });
        }

        [HttpDelete("document/{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _documentRepo.FindByIdAsync(id);
            if (document == null) return NotFound();
            if (!await CanAccessProfile(document.CandidateId)) return Forbid();

            _fileService.DeleteFile(document.FilePath);

            await _documentRepo.DeleteAsync(id);
            await _documentRepo.SaveChangesAsync();
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? "";
        }

        private async Task<Candidate?> GetCandidateForCurrentUser()
        {
            var currentUserId = GetCurrentUserId();
            return await _context.Candidates
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == currentUserId);
        }

        private async Task<bool> CanAccessProfile(int candidateId)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserRole == "Admin" || currentUserRole == "Recruiter" || currentUserRole == "HR" || currentUserRole == "Interviewer")
            {
                return true;
            }

            if (currentUserRole == "Candidate")
            {
                var candidate = await _context.Candidates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == candidateId && c.UserId == currentUserId);
                return candidate != null;
            }

            return false;
        }
    }
}
