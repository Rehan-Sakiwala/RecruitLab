using recruitlab.server.Data;
using recruitlab.server.Services.Interface;
using Server.Model.DTO;
using Server.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace recruitlab.server.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public UserService(AppDbContext context, IAuthService authService, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _authService = authService;
            _emailService = emailService;
            _config = config;
        }

        public async Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return (false, "Invalid email or password.", null);

            if (user.Status == AccountStatus.Pending)
                return (false, "Account is not active. Please check your email for an OTP.", null);

            if (!_authService.VerifyPasswordHash(dto.Password, user.PasswordHash))
                return (false, "Invalid email or password.", null);

            var response = new LoginResponseDto
            {
                Token = _authService.CreateJwtToken(user),
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.Name
            };

            return (true, "Login successful.", response);
        }

        public async Task<(bool Success, string Message)> RegisterCandidateAsync(CandidateSelfRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return (false, "An account with this email already exists.");
            }

            string passwordHash = _authService.CreatePasswordHash(dto.Password);
            string otp = _authService.GenerateOtp();

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Candidate");
            if (role == null) return (false, "Critical error: Candidate role not found.");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                Status = AccountStatus.Pending,
                VerificationOtp = otp,
                OtpExpiry = DateTime.UtcNow.AddMinutes(15)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var candidate = new Candidate
            {
                UserId = user.Id,
                CreatedByUserId = user.Id,
                Source = CandidateSource.SelfRegistered
            };

            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();

            var emailBody = $"Welcome to RecruitLab!<br>Your verification code is: <strong>{otp}</strong><br>This code will expire in 15 minutes.";
            await _emailService.SendEmailAsync(user.Email, "Verify Your Email Address", emailBody);

            return (true, "Registration successful. Please check your email for an OTP.");
        }

        public async Task<(bool Success, string Message, LoginResponseDto? Data)> VerifyCandidateEmailAsync(VerifyOtpDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null) return (false, "User not found.", null);
            if (user.Status == AccountStatus.Active) return (false, "Email already verified.", null);
            if (user.OtpExpiry < DateTime.UtcNow) return (false, "OTP has expired.", null);
            if (user.VerificationOtp != dto.Otp) return (false, "Invalid OTP.", null);

            user.Status = AccountStatus.Active;
            user.VerificationOtp = null;
            user.OtpExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var response = new LoginResponseDto
            {
                Token = _authService.CreateJwtToken(user),
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.Name
            };

            return (true, "Email verified successfully.", response);
        }

        public async Task<(bool Success, string Message)> CreateCandidateByRecruiterAsync(RecruiterCreateCandidateDto dto, int recruiterId)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return (false, "An account with this email already exists.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Candidate");
            if (role == null) return (false, "Critical error: Candidate role not found.");

            string tempPassword = _authService.GenerateRandomPassword();
            string passwordHash = _authService.CreatePasswordHash(tempPassword);
            string resetToken = _authService.CreatePasswordResetToken();

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                Status = AccountStatus.CreatedByRecruiter,
                PasswordResetToken = resetToken,
                ResetTokenExpiry = DateTime.UtcNow.AddDays(3)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var candidate = new Candidate
            {
                UserId = user.Id,
                CreatedByUserId = recruiterId,
                Source = CandidateSource.Manual,
                ExpectedSalary = dto.ExpectedSalary
            };

            await _context.Candidates.AddAsync(candidate);
            await _context.SaveChangesAsync();

            var frontendUrl = _config["App:FrontendUrl"] ?? "http://localhost:3000";
            var changePasswordLink = $"{frontendUrl}/set-password?token={resetToken}&email={user.Email}";

            var emailBody = $"Welcome to RecruitLab!<br>A recruiter has created an account for you.<br><br>" +
                            $"Your login email is: <strong>{user.Email}</strong><br>" +
                            $"Your temporary password is: <strong>{tempPassword}</strong><br><br>" +
                            $"Please <a href='{changePasswordLink}'>click this link to set your password and activate your account</a>.";

            await _emailService.SendEmailAsync(user.Email, "Your New RecruitLab Account", emailBody);

            return (true, "Candidate created successfully and credentials sent via email.");
        }

        public async Task<(bool Success, string Message)> SetPasswordFromTokenAsync(SetPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return (false, "Invalid user or token.");

            if (user.PasswordResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
                return (false, "Invalid or expired token.");

            user.PasswordHash = _authService.CreatePasswordHash(dto.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiry = null;
            user.Status = AccountStatus.Active; // Activate the account
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return (true, "Password has been reset successfully. You can now log in.");
        }
    }
}
