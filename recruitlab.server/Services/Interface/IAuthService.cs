using Server.Model.Entities;

namespace recruitlab.server.Services.Interface
{
    public interface IAuthService
    {
        string CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, string hash);
        string CreateJwtToken(User user);
        string GenerateRandomPassword(int length = 12);
        string GenerateOtp();
        string CreatePasswordResetToken();
    }
}
