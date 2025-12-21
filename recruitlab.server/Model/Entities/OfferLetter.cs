namespace recruitlab.server.Model.Entities
{
    public class OfferLetter
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;

        public string JobType { get; set; } = null!;
        public string Compensation { get; set; } = null!;
        public string WorkLocation { get; set; } = null!;
        public string ReportingTo { get; set; } = null!;

        public DateTime OfferDate { get; set; }
        public DateTime? JoiningDate { get; set; }

        public string FilePath { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
