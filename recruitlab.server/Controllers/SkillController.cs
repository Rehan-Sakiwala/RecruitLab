using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using recruitlab.server.Data;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SkillController : ControllerBase
    {
        private readonly IRepository<Skill> skillRepo;
        private readonly IRepository<SkillCategory> skillCategoryRepo;
        private readonly AppDbContext dbContext;

        public SkillController(
            IRepository<Skill> skillRepo,
            IRepository<SkillCategory> skillCategoryRepo,
            AppDbContext dbContext)
        {
            this.skillRepo = skillRepo;
            this.skillCategoryRepo = skillCategoryRepo;
            this.dbContext = dbContext;
        }

        #region Skill Categories

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllSkillCategories()
        {
            var categories = await skillCategoryRepo.GetAll();
            var categoryDtos = categories.Select(MapToSkillCategoryDto).ToList();
            return Ok(categoryDtos);
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetSkillCategory(int id)
        {
            var category = await skillCategoryRepo.FindByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Skill category not found" });

            return Ok(MapToSkillCategoryDto(category));
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateSkillCategory([FromBody] CreateSkillCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = new SkillCategory
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await skillCategoryRepo.AddAsync(category);
            await skillCategoryRepo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSkillCategory), new { id = category.Id }, MapToSkillCategoryDto(category));
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateSkillCategory(int id, [FromBody] UpdateSkillCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await skillCategoryRepo.FindByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Skill category not found" });

            category.Name = dto.Name;
            category.Description = dto.Description;

            skillCategoryRepo.Update(category);
            await skillCategoryRepo.SaveChangesAsync();

            return Ok(MapToSkillCategoryDto(category));
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteSkillCategory(int id)
        {
            var category = await skillCategoryRepo.FindByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Skill category not found" });

            var skillsInCategory = await dbContext.Skills
                .Where(s => s.SkillCategoryId == id)
                .CountAsync();

            if (skillsInCategory > 0)
                return BadRequest(new { message = "Cannot delete category that contains skills" });

            await skillCategoryRepo.DeleteAsync(id);
            await skillCategoryRepo.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        #region Skills

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await dbContext.Skills
                .Include(s => s.SkillCategory)
                .ToListAsync();

            var skillDtos = skills.Select(MapToSkillDto).ToList();
            return Ok(skillDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkill(int id)
        {
            var skill = await dbContext.Skills
                .Include(s => s.SkillCategory)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                return NotFound(new { message = "Skill not found" });

            return Ok(MapToSkillDto(skill));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetSkillsByCategory(int categoryId)
        {
            var skills = await dbContext.Skills
                .Include(s => s.SkillCategory)
                .Where(s => s.SkillCategoryId == categoryId)
                .ToListAsync();

            var skillDtos = skills.Select(MapToSkillDto).ToList();
            return Ok(skillDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await skillCategoryRepo.FindByIdAsync(dto.SkillCategoryId);
            if (category == null)
                return BadRequest(new { message = "Skill category not found" });

            var skill = new Skill
            {
                Name = dto.Name,
                Description = dto.Description,
                SkillCategoryId = dto.SkillCategoryId
            };

            await skillRepo.AddAsync(skill);
            await skillRepo.SaveChangesAsync();

            var createdSkill = await dbContext.Skills
                .Include(s => s.SkillCategory)
                .FirstOrDefaultAsync(s => s.Id == skill.Id);

            return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, MapToSkillDto(createdSkill));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] UpdateSkillDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var skill = await skillRepo.FindByIdAsync(id);
            if (skill == null)
                return NotFound(new { message = "Skill not found" });

            var category = await skillCategoryRepo.FindByIdAsync(dto.SkillCategoryId);
            if (category == null)
                return BadRequest(new { message = "Skill category not found" });

            skill.Name = dto.Name;
            skill.Description = dto.Description;
            skill.SkillCategoryId = dto.SkillCategoryId;

            skillRepo.Update(skill);
            await skillRepo.SaveChangesAsync();

            var updatedSkill = await dbContext.Skills
                .Include(s => s.SkillCategory)
                .FirstOrDefaultAsync(s => s.Id == id);

            return Ok(MapToSkillDto(updatedSkill));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await skillRepo.FindByIdAsync(id);
            if (skill == null)
                return NotFound(new { message = "Skill not found" });

            var jobsUsingSkill = await dbContext.JobSkills
                .Where(js => js.SkillId == id)
                .CountAsync();

            if (jobsUsingSkill > 0)
                return BadRequest(new { message = "Cannot delete skill that is used in job openings" });

            await skillRepo.DeleteAsync(id);
            await skillRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchSkills([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Search query is required" });

            var skills = await dbContext.Skills
                .Include(s => s.SkillCategory)
                .Where(s => s.Name.Contains(query) || s.Description.Contains(query))
                .ToListAsync();

            var skillDtos = skills.Select(MapToSkillDto).ToList();
            return Ok(skillDtos);
        }

        #endregion

        private SkillCategoryDto MapToSkillCategoryDto(SkillCategory category)
        {
            return new SkillCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt
            };
        }

        private SkillDto MapToSkillDto(Skill skill)
        {
            return new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name,
                Description = skill.Description,
                SkillCategoryId = skill.SkillCategoryId,
                SkillCategoryName = skill.SkillCategory?.Name ?? "",
                CreatedAt = skill.CreatedAt
            };
        }
    }
}
