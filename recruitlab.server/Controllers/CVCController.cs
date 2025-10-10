using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CVCController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<CandidateCV> _cvRepository;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly ICVProcessingService _cvProcessingService;

        public CVCController(
            AppDbContext context,
            IRepository<CandidateCV> cvRepository,
            IRepository<Candidate> candidateRepository,
            ICVProcessingService cvProcessingService)
        {
            _context = context;
            _cvRepository = cvRepository;
            _candidateRepository = candidateRepository;
            _cvProcessingService = cvProcessingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCVs(
            [FromQuery] int? candidateId = null,
            [FromQuery] int? status = null,
            [FromQuery] int? type = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.CandidateCVs
                .Include(cv => cv.Candidate)
                .Include(cv => cv.UploadedByUser)
                .AsQueryable();

            // Apply filters
            if (candidateId.HasValue)
                query = query.Where(cv => cv.CandidateId == candidateId.Value);

            if (status.HasValue)
                query = query.Where(cv => (int)cv.Status == status.Value);

            if (type.HasValue)
                query = query.Where(cv => (int)cv.Type == type.Value);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(cv => 
                    cv.FileName.Contains(search) ||
                    cv.Candidate.FirstName.Contains(search) ||
                    cv.Candidate.LastName.Contains(search) ||
                    cv.Candidate.Email.Contains(search));
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var cvs = await query
                .OrderByDescending(cv => cv.UploadedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var cvDtos = cvs.Select(MapToCVDto).ToList();

            return Ok(new
            {
                CVs = cvDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCV(int id)
        {
            var cv = await _context.CandidateCVs
                .Include(cv => cv.Candidate)
                .Include(cv => cv.UploadedByUser)
                .FirstOrDefaultAsync(cv => cv.Id == id);

            if (cv == null)
                return NotFound(new { message = "CV not found" });

            return Ok(MapToCVDto(cv));
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCV([FromForm] CVUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _cvProcessingService.ProcessCVAsync(dto.File, userId.Value);
            
            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("upload-for-candidate/{candidateId}")]
        public async Task<IActionResult> UploadCVForCandidate(int candidateId, [FromForm] CVUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var candidate = await _candidateRepository.FindByIdAsync(candidateId);
            if (candidate == null)
                return NotFound(new { message = "Candidate not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Save CV file
            var fileExtension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
            var fileName = $"{candidateId}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            var filePath = Path.Combine("uploads", "cvs", fileName);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            // Create CV record
            var candidateCV = new CandidateCV
            {
                CandidateId = candidateId,
                FileName = dto.File.FileName,
                FilePath = filePath,
                FileType = fileExtension,
                FileSize = dto.File.Length,
                Type = (CVType)dto.Type,
                Status = CVStatus.Uploaded,
                ProcessingNotes = dto.ProcessingNotes,
                UploadedByUserId = userId.Value
            };

            await _cvRepository.AddAsync(candidateCV);
            await _cvRepository.SaveChangesAsync();

            return Ok(MapToCVDto(candidateCV));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCV(int id, [FromBody] UpdateCandidateCVDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cv = await _cvRepository.FindByIdAsync(id);
            if (cv == null)
                return NotFound(new { message = "CV not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            cv.Type = (CVType)dto.Type;
            cv.Status = (CVStatus)dto.Status;
            cv.ParsedContent = dto.ParsedContent;
            cv.ParsedSkills = dto.ParsedSkills;
            cv.ParsedExperience = dto.ParsedExperience;
            cv.ParsedEducation = dto.ParsedEducation;
            cv.ProcessingNotes = dto.ProcessingNotes;
            cv.IsActive = dto.IsActive;

            if (dto.Status == (int)CVStatus.Processed)
                cv.ProcessedAt = DateTime.UtcNow;

            _cvRepository.Update(cv);
            await _cvRepository.SaveChangesAsync();

            var updatedCV = await _context.CandidateCVs
                .Include(cv => cv.Candidate)
                .Include(cv => cv.UploadedByUser)
                .FirstOrDefaultAsync(cv => cv.Id == id);

            return Ok(MapToCVDto(updatedCV));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCV(int id)
        {
            var cv = await _cvRepository.FindByIdAsync(id);
            if (cv == null)
                return NotFound(new { message = "CV not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Soft delete
            cv.IsActive = false;
            _cvRepository.Update(cv);
            await _cvRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadCV(int id)
        {
            var cv = await _cvRepository.FindByIdAsync(id);
            if (cv == null)
                return NotFound(new { message = "CV not found" });

            if (!System.IO.File.Exists(cv.FilePath))
                return NotFound(new { message = "CV file not found on disk" });

            var fileBytes = await System.IO.File.ReadAllBytesAsync(cv.FilePath);
            return File(fileBytes, GetContentType(cv.FileType), cv.FileName);
        }

        [HttpPost("{id}/reprocess")]
        public async Task<IActionResult> ReprocessCV(int id)
        {
            var cv = await _cvRepository.FindByIdAsync(id);
            if (cv == null)
                return NotFound(new { message = "CV not found" });

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            // Update status to processing
            cv.Status = CVStatus.Processing;
            cv.ProcessingNotes = "Reprocessing requested";
            _cvRepository.Update(cv);
            await _cvRepository.SaveChangesAsync();

            try
            {
                // Simulate reprocessing (in real implementation, you would call the CV processing service)
                await Task.Delay(2000); // Simulate processing time

                cv.Status = CVStatus.Processed;
                cv.ProcessedAt = DateTime.UtcNow;
                cv.ProcessingNotes = "Reprocessed successfully";
                _cvRepository.Update(cv);
                await _cvRepository.SaveChangesAsync();

                return Ok(new { message = "CV reprocessed successfully" });
            }
            catch (Exception ex)
            {
                cv.Status = CVStatus.Failed;
                cv.ProcessingNotes = $"Reprocessing failed: {ex.Message}";
                _cvRepository.Update(cv);
                await _cvRepository.SaveChangesAsync();

                return BadRequest(new { message = "Failed to reprocess CV", error = ex.Message });
            }
        }

        [HttpGet("status-counts")]
        public async Task<IActionResult> GetCVStatusCounts()
        {
            var counts = await _context.CandidateCVs
                .Where(cv => cv.IsActive)
                .GroupBy(cv => cv.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    StatusId = (int)g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(counts);
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return null;
        }

        private CandidateCVDto MapToCVDto(CandidateCV cv)
        {
            return new CandidateCVDto
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
            };
        }

        private string GetContentType(string fileType)
        {
            return fileType.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
