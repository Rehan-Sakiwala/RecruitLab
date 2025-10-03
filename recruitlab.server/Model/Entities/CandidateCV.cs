namespace Server.Model.Entities
{
    public enum CVType
    {
        Original = 1,
        Processed = 2,
        Parsed = 3
    }

    public enum CVStatus
    {
        Uploaded = 1,
        Processing = 2,
        Processed = 3,
        Failed = 4,
        Parsed = 5
    }

    public class CandidateCV
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; } // pdf, doc, docx, etc.
        public long FileSize { get; set; }
        public CVType Type { get; set; } = CVType.Original;
        public CVStatus Status { get; set; } = CVStatus.Uploaded;
        public string? ParsedContent { get; set; }
        public string? ParsedSkills { get; set; }
        public string? ParsedExperience { get; set; }
        public string? ParsedEducation { get; set; }
        public string? ProcessingNotes { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public int UploadedByUserId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Candidate Candidate { get; set; }
        public User UploadedByUser { get; set; }
    }
}
