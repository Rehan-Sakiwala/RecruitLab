using Server.Model.DTO;

namespace Server.Services
{
    public interface ICandidateMatchingService
    {
        Task<List<CandidateMatchDto>> FindMatchingCandidatesAsync(JobMatchRequestDto request);
        Task<List<CandidateMatchDto>> FindMatchingCandidatesForJobAsync(int jobOpeningId, decimal minMatchScore = 0.5m, bool includeInactive = false, int? limit = null);
        Task<decimal> CalculateMatchScoreAsync(int candidateId, int jobOpeningId);
        Task<CandidateJobMatchDto> CreateCandidateJobMatchAsync(int candidateId, int jobOpeningId, int matchedByUserId, MatchType type = MatchType.Automatic);
        Task<List<CandidateMatchDto>> SearchCandidatesByProfileAsync(ProfileSearchDto searchDto);
    }
}
