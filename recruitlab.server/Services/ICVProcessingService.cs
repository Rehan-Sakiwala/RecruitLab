using Server.Model.DTO;

namespace Server.Services
{
    public interface ICVProcessingService
    {
        Task<CVProcessingResultDto> ProcessCVAsync(IFormFile cvFile, int uploadedByUserId);
        Task<CandidateProfileDto?> ExtractProfileFromCVAsync(string cvContent, string fileName);
        Task<List<string>> ExtractSkillsFromTextAsync(string text);
        Task<string?> ExtractExperienceFromTextAsync(string text);
        Task<string?> ExtractEducationFromTextAsync(string text);
        Task<List<string>> ExtractCertificationsFromTextAsync(string text);
        Task<string> ExtractEmailFromTextAsync(string text);
        Task<string?> ExtractPhoneFromTextAsync(string text);
    }
}
