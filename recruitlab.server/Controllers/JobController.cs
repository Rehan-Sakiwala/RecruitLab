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
    public class JobController : ControllerBase
    {
        private readonly IRepository<JobOpening> jobRepo;
        private readonly IRepository<JobSkill> jobSkillRepo;
        private readonly IRepository<Skill> skillRepo;
        private readonly IRepository<User> userRepo;
        private readonly AppDbContext dbContext;

        public JobController(
            IRepository<JobOpening> jobRepo,
            IRepository<JobSkill> jobSkillRepo,
            IRepository<Skill> skillRepo,
            IRepository<User> userRepo,
            AppDbContext dbContext)
        {
            this.jobRepo = jobRepo;
            this.jobSkillRepo = jobSkillRepo;
            this.skillRepo = skillRepo;
            this.userRepo = userRepo;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await dbContext.JobOpenings
                .Include(j => j.CreatedByUser)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .ToListAsync();

            var jobDtos = jobs.Select(MapToJobOpeningDto).ToList();
            return Ok(jobDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var job = await dbContext.JobOpenings
                .Include(j => j.CreatedByUser)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return NotFound(new { message = "Job opening not found" });

            return Ok(MapToJobOpeningDto(job));
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobOpeningDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var job = new JobOpening
            {
                Title = dto.Title,
                Description = dto.Description,
                Department = dto.Department,
                Location = dto.Location,
                SalaryMin = dto.SalaryMin,
                SalaryMax = dto.SalaryMax,
                CreatedByUserId = userId.Value,
                Status = JobStatus.Open
            };

            await jobRepo.AddAsync(job);
            await jobRepo.SaveChangesAsync();

            foreach (var skillDto in dto.JobSkills)
            {
                var jobSkill = new JobSkill
                {
                    JobOpeningId = job.Id,
                    SkillId = skillDto.SkillId,
                    RequirementType = (SkillRequirementType)skillDto.RequirementType,
                    YearsOfExperience = skillDto.YearsOfExperience,
                    Notes = skillDto.Notes
                };
                await jobSkillRepo.AddAsync(jobSkill);
            }

            await jobRepo.SaveChangesAsync();

            var createdJob = await dbContext.JobOpenings
                .Include(j => j.CreatedByUser)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .FirstOrDefaultAsync(j => j.Id == job.Id);

            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, MapToJobOpeningDto(createdJob));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobOpeningDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = await jobRepo.FindByIdAsync(id);
            if (job == null)
                return NotFound(new { message = "Job opening not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var userRole = GetCurrentUserRole();
            if (job.CreatedByUserId != userId.Value && userRole != "Admin")
                return Forbid();

            job.Title = dto.Title;
            job.Description = dto.Description;
            job.Department = dto.Department;
            job.Location = dto.Location;
            job.SalaryMin = dto.SalaryMin;
            job.SalaryMax = dto.SalaryMax;
            job.Status = (JobStatus)dto.Status;
            job.StatusReason = dto.StatusReason;
            job.UpdatedAt = DateTime.UtcNow;

            jobRepo.Update(job);
            await jobRepo.SaveChangesAsync();

            var updatedJob = await dbContext.JobOpenings
                .Include(j => j.CreatedByUser)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .FirstOrDefaultAsync(j => j.Id == id);

            return Ok(MapToJobOpeningDto(updatedJob));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await jobRepo.FindByIdAsync(id);
            if (job == null)
                return NotFound(new { message = "Job opening not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var userRole = GetCurrentUserRole();
            if (job.CreatedByUserId != userId.Value && userRole != "Admin")
                return Forbid();

            await jobRepo.DeleteAsync(id);
            await jobRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/skills")]
        public async Task<IActionResult> AddSkillToJob(int id, [FromBody] CreateJobSkillDto dto)
        {
            var job = await jobRepo.FindByIdAsync(id);
            if (job == null)
                return NotFound(new { message = "Job opening not found" });

            var skill = await skillRepo.FindByIdAsync(dto.SkillId);
            if (skill == null)
                return NotFound(new { message = "Skill not found" });

            var existingJobSkill = await dbContext.JobSkills
                .FirstOrDefaultAsync(js => js.JobOpeningId == id && js.SkillId == dto.SkillId);

            if (existingJobSkill != null)
                return BadRequest(new { message = "Skill already added to this job" });

            var jobSkill = new JobSkill
            {
                JobOpeningId = id,
                SkillId = dto.SkillId,
                RequirementType = (SkillRequirementType)dto.RequirementType,
                YearsOfExperience = dto.YearsOfExperience,
                Notes = dto.Notes
            };

            await jobSkillRepo.AddAsync(jobSkill);
            await jobSkillRepo.SaveChangesAsync();

            return Ok(new { message = "Skill added to job successfully" });
        }

        [HttpPut("{id}/skills/{skillId}")]
        public async Task<IActionResult> UpdateJobSkill(int id, int skillId, [FromBody] UpdateJobSkillDto dto)
        {
            var jobSkill = await dbContext.JobSkills
                .FirstOrDefaultAsync(js => js.JobOpeningId == id && js.SkillId == skillId);

            if (jobSkill == null)
                return NotFound(new { message = "Job skill not found" });

            jobSkill.RequirementType = (SkillRequirementType)dto.RequirementType;
            jobSkill.YearsOfExperience = dto.YearsOfExperience;
            jobSkill.Notes = dto.Notes;

            jobSkillRepo.Update(jobSkill);
            await jobSkillRepo.SaveChangesAsync();

            return Ok(new { message = "Job skill updated successfully" });
        }

        [HttpDelete("{id}/skills/{skillId}")]
        public async Task<IActionResult> RemoveSkillFromJob(int id, int skillId)
        {
            var jobSkill = await dbContext.JobSkills
                .FirstOrDefaultAsync(js => js.JobOpeningId == id && js.SkillId == skillId);

            if (jobSkill == null)
                return NotFound(new { message = "Job skill not found" });

            await jobSkillRepo.DeleteAsync(jobSkill.Id);
            await jobSkillRepo.SaveChangesAsync();

            return Ok(new { message = "Skill removed from job successfully" });
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetJobsByStatus(int status)
        {
            var jobs = await dbContext.JobOpenings
                .Include(j => j.CreatedByUser)
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .Where(j => (int)j.Status == status)
                .ToListAsync();

            var jobDtos = jobs.Select(MapToJobOpeningDto).ToList();
            return Ok(jobDtos);
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

        private JobOpeningDto MapToJobOpeningDto(JobOpening job)
        {
            return new JobOpeningDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Department = job.Department,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Status = (int)job.Status,
                StatusName = job.Status.ToString(),
                StatusReason = job.StatusReason,
                CreatedByUserId = job.CreatedByUserId,
                CreatedByUserEmail = job.CreatedByUser?.Email ?? "",
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt,
                JobSkills = job.JobSkills.Select(js => new JobSkillDto
                {
                    Id = js.Id,
                    SkillId = js.SkillId,
                    SkillName = js.Skill?.Name ?? "",
                    SkillCategoryName = js.Skill?.SkillCategory?.Name ?? "",
                    RequirementType = (int)js.RequirementType,
                    RequirementTypeName = js.RequirementType.ToString(),
                    YearsOfExperience = js.YearsOfExperience,
                    Notes = js.Notes
                }).ToList()
            };
        }
    }
}
