using Microsoft.AspNetCore.Http;

namespace Server.Model.DTO
{
    public class CandidateCVDto
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string? ParsedContent { get; set; }
        public string? ParsedSkills { get; set; }
        public string? ParsedExperience { get; set; }
        public string? ParsedEducation { get; set; }
        public string? ProcessingNotes { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int UploadedByUserId { get; set; }
        public string UploadedByUserEmail { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCandidateCVDto
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public int Type { get; set; } = 1;
        public string? ProcessingNotes { get; set; }
    }

    public class UpdateCandidateCVDto
    {
        public int Type { get; set; }
        public int Status { get; set; }
        public string? ParsedContent { get; set; }
        public string? ParsedSkills { get; set; }
        public string? ParsedExperience { get; set; }
        public string? ParsedEducation { get; set; }
        public string? ProcessingNotes { get; set; }
        public bool IsActive { get; set; }
    }

    public class CVUploadDto
    {
        public IFormFile File { get; set; }
        public int Type { get; set; } = 1;
        public string? ProcessingNotes { get; set; }
    }
}
