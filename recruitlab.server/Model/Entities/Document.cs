namespace Server.Model.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DocumentType Type { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }

    public enum DocumentType
    {
        CV,
        AdressProof,
        PAN,
        Bank,
        Certification,
        Other
    }
}