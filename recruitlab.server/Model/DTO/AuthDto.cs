using System.ComponentModel.DataAnnotations;

namespace Server.Model.DTO
{
    public class CandidateSelfRegisterDto
    {
        [Required, MinLength(2)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MinLength(2)]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class RecruiterCreateCandidateDto
    {
        [Required, MinLength(2)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MinLength(2)]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public decimal? ExpectedSalary { get; set; }
    }

    public class VerifyOtpDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, Length(6, 6)]
        public string Otp { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class SetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
