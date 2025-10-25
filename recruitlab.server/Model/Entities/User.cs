namespace Server.Model.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public AccountStatus Status { get; set; } = AccountStatus.Pending;
        public string? VerificationOtp { get; set; }
        public DateTime? OtpExpiry { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Candidate? CandidateProfile { get; set; }
    }

    public enum AccountStatus
    {
        Pending,
        Active,
        CreatedByRecruiter,
        Disabled
    }
}