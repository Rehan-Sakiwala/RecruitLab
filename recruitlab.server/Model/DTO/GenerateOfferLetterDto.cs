namespace recruitlab.server.Model.DTO
{
    public class GenerateOfferLetterDto
    {
        public int ApplicationId { get; set; }
        public string JobType { get; set; } = null!;
        public string Compensation { get; set; } = null!;
        public string WorkLocation { get; set; } = null!;
        public string ReportingTo { get; set; } = null!;
        public DateTime? JoiningDate { get; set; }
    }
}
