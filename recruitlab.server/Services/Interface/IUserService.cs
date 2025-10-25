using Server.Model.DTO;

namespace recruitlab.server.Services.Interface
{
    public interface IUserService
    {
        Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(LoginDto dto);
        Task<(bool Success, string Message)> RegisterCandidateAsync(CandidateSelfRegisterDto dto);
        Task<(bool Success, string Message, LoginResponseDto? Data)> VerifyCandidateEmailAsync(VerifyOtpDto dto);
        Task<(bool Success, string Message)> CreateCandidateByRecruiterAsync(RecruiterCreateCandidateDto dto, int recruiterId);
        Task<(bool Success, string Message)> SetPasswordFromTokenAsync(SetPasswordDto dto);
    }
}
