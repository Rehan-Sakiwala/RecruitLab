using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;

namespace Server.Services
{
    public class CandidateMatchingService : ICandidateMatchingService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<CandidateJobMatch> _candidateJobMatchRepository;

        public CandidateMatchingService(
            AppDbContext context,
            IRepository<CandidateJobMatch> candidateJobMatchRepository)
        {
            _context = context;
            _candidateJobMatchRepository = candidateJobMatchRepository;
        }

        public async Task<List<CandidateMatchDto>> FindMatchingCandidatesAsync(JobMatchRequestDto request)
        {
            return await FindMatchingCandidatesForJobAsync(
                request.JobOpeningId, 
                request.MinMatchScore, 
                request.IncludeInactiveCandidates, 
                request.Limit);
        }

        public async Task<List<CandidateMatchDto>> FindMatchingCandidatesForJobAsync(int jobOpeningId, decimal minMatchScore = 0.5m, bool includeInactive = false, int? limit = null)
        {
            var jobOpening = await _context.JobOpenings
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .FirstOrDefaultAsync(j => j.Id == jobOpeningId);

            if (jobOpening == null)
                return new List<CandidateMatchDto>();

            var jobSkills = jobOpening.JobSkills.Select(js => js.SkillId).ToList();
            if (!jobSkills.Any())
                return new List<CandidateMatchDto>();

            var query = _context.Candidates
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                .Include(c => c.CandidateJobMatches.Where(cjm => cjm.JobOpeningId == jobOpeningId))
                .Where(c => c.Status == CandidateStatus.Active);

            if (!includeInactive)
                query = query.Where(c => c.IsAvailable);

            var candidates = await query.ToListAsync();
            var matches = new List<CandidateMatchDto>();

            foreach (var candidate in candidates)
            {
                var candidateSkills = candidate.CandidateSkills.Select(cs => cs.SkillId).ToList();
                var matchingSkills = jobSkills.Intersect(candidateSkills).ToList();
                var missingSkills = jobSkills.Except(candidateSkills).ToList();

                if (!matchingSkills.Any())
                    continue;

                var matchScore = CalculateMatchScore(matchingSkills.Count, jobSkills.Count, candidateSkills.Count);
                
                if (matchScore >= minMatchScore)
                {
                    var match = new CandidateMatchDto
                    {
                        CandidateId = candidate.Id,
                        CandidateName = $"{candidate.FirstName} {candidate.LastName}",
                        CandidateEmail = candidate.Email,
                        MatchScore = matchScore,
                        MatchingSkills = candidate.CandidateSkills
                            .Where(cs => matchingSkills.Contains(cs.SkillId))
                            .Select(cs => cs.Skill.Name)
                            .ToList(),
                        MissingSkills = jobOpening.JobSkills
                            .Where(js => missingSkills.Contains(js.SkillId))
                            .Select(js => js.Skill.Name)
                            .ToList(),
                        Notes = $"Matched {matchingSkills.Count} out of {jobSkills.Count} required skills"
                    };

                    matches.Add(match);
                }
            }

            // Sort by match score descending
            matches = matches.OrderByDescending(m => m.MatchScore).ToList();

            if (limit.HasValue)
                matches = matches.Take(limit.Value).ToList();

            return matches;
        }

        public async Task<decimal> CalculateMatchScoreAsync(int candidateId, int jobOpeningId)
        {
            var candidate = await _context.Candidates
                .Include(c => c.CandidateSkills)
                .FirstOrDefaultAsync(c => c.Id == candidateId);

            var jobOpening = await _context.JobOpenings
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == jobOpeningId);

            if (candidate == null || jobOpening == null)
                return 0;

            var candidateSkills = candidate.CandidateSkills.Select(cs => cs.SkillId).ToList();
            var jobSkills = jobOpening.JobSkills.Select(js => js.SkillId).ToList();

            if (!jobSkills.Any())
                return 0;

            var matchingSkills = jobSkills.Intersect(candidateSkills).ToList();
            return CalculateMatchScore(matchingSkills.Count, jobSkills.Count, candidateSkills.Count);
        }

        public async Task<CandidateJobMatchDto> CreateCandidateJobMatchAsync(int candidateId, int jobOpeningId, int matchedByUserId, MatchType type = MatchType.Automatic)
        {
            // Check if match already exists
            var existingMatch = await _context.CandidateJobMatches
                .FirstOrDefaultAsync(cjm => cjm.CandidateId == candidateId && cjm.JobOpeningId == jobOpeningId);

            if (existingMatch != null)
            {
                return new CandidateJobMatchDto
                {
                    Id = existingMatch.Id,
                    CandidateId = existingMatch.CandidateId,
                    JobOpeningId = existingMatch.JobOpeningId,
                    Status = (int)existingMatch.Status,
                    StatusName = existingMatch.Status.ToString(),
                    Type = (int)existingMatch.Type,
                    TypeName = existingMatch.Type.ToString(),
                    MatchScore = existingMatch.MatchScore,
                    MatchDetails = existingMatch.MatchDetails,
                    Notes = existingMatch.Notes,
                    MatchedAt = existingMatch.MatchedAt,
                    StatusUpdatedAt = existingMatch.StatusUpdatedAt,
                    MatchedByUserId = existingMatch.MatchedByUserId,
                    InterviewScheduledAt = existingMatch.InterviewScheduledAt,
                    InterviewNotes = existingMatch.InterviewNotes,
                    OfferedSalary = existingMatch.OfferedSalary,
                    OfferDate = existingMatch.OfferDate,
                    ResponseDeadline = existingMatch.ResponseDeadline,
                    IsActive = existingMatch.IsActive
                };
            }

            var matchScore = await CalculateMatchScoreAsync(candidateId, jobOpeningId);
            
            var candidateJobMatch = new CandidateJobMatch
            {
                CandidateId = candidateId,
                JobOpeningId = jobOpeningId,
                Status = MatchStatus.Pending,
                Type = type,
                MatchScore = matchScore,
                MatchDetails = $"Auto-generated match with score: {matchScore:P2}",
                MatchedByUserId = matchedByUserId,
                MatchedAt = DateTime.UtcNow
            };

            await _candidateJobMatchRepository.AddAsync(candidateJobMatch);
            await _candidateJobMatchRepository.SaveChangesAsync();

            var createdMatch = await _context.CandidateJobMatches
                .Include(cjm => cjm.Candidate)
                .Include(cjm => cjm.JobOpening)
                .Include(cjm => cjm.MatchedByUser)
                .FirstOrDefaultAsync(cjm => cjm.Id == candidateJobMatch.Id);

            return new CandidateJobMatchDto
            {
                Id = createdMatch.Id,
                CandidateId = createdMatch.CandidateId,
                CandidateName = $"{createdMatch.Candidate.FirstName} {createdMatch.Candidate.LastName}",
                CandidateEmail = createdMatch.Candidate.Email,
                JobOpeningId = createdMatch.JobOpeningId,
                JobTitle = createdMatch.JobOpening.Title,
                JobDepartment = createdMatch.JobOpening.Department,
                Status = (int)createdMatch.Status,
                StatusName = createdMatch.Status.ToString(),
                Type = (int)createdMatch.Type,
                TypeName = createdMatch.Type.ToString(),
                MatchScore = createdMatch.MatchScore,
                MatchDetails = createdMatch.MatchDetails,
                Notes = createdMatch.Notes,
                MatchedAt = createdMatch.MatchedAt,
                StatusUpdatedAt = createdMatch.StatusUpdatedAt,
                MatchedByUserId = createdMatch.MatchedByUserId,
                MatchedByUserEmail = createdMatch.MatchedByUser?.Email ?? "",
                InterviewScheduledAt = createdMatch.InterviewScheduledAt,
                InterviewNotes = createdMatch.InterviewNotes,
                OfferedSalary = createdMatch.OfferedSalary,
                OfferDate = createdMatch.OfferDate,
                ResponseDeadline = createdMatch.ResponseDeadline,
                IsActive = createdMatch.IsActive
            };
        }

        public async Task<List<CandidateMatchDto>> SearchCandidatesByProfileAsync(ProfileSearchDto searchDto)
        {
            var query = _context.Candidates
                .Include(c => c.CreatedByUser)
                .Include(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
                        .ThenInclude(s => s.SkillCategory)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
            {
                query = query.Where(c => 
                    c.FirstName.Contains(searchDto.SearchTerm) ||
                    c.LastName.Contains(searchDto.SearchTerm) ||
                    c.Email.Contains(searchDto.SearchTerm) ||
                    c.CurrentPosition.Contains(searchDto.SearchTerm) ||
                    c.CurrentCompany.Contains(searchDto.SearchTerm) ||
                    c.ExperienceSummary.Contains(searchDto.SearchTerm));
            }

            if (searchDto.SkillIds.Any())
            {
                query = query.Where(c => c.CandidateSkills.Any(cs => searchDto.SkillIds.Contains(cs.SkillId)));
            }

            if (searchDto.SkillCategoryIds.Any())
            {
                query = query.Where(c => c.CandidateSkills.Any(cs => 
                    searchDto.SkillCategoryIds.Contains(cs.Skill.SkillCategoryId)));
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Location))
            {
                query = query.Where(c => 
                    c.City.Contains(searchDto.Location) ||
                    c.State.Contains(searchDto.Location) ||
                    c.Country.Contains(searchDto.Location));
            }

            if (searchDto.MinSalary.HasValue)
            {
                query = query.Where(c => c.ExpectedSalary >= searchDto.MinSalary.Value);
            }

            if (searchDto.MaxSalary.HasValue)
            {
                query = query.Where(c => c.ExpectedSalary <= searchDto.MaxSalary.Value);
            }

            if (searchDto.IsAvailable)
            {
                query = query.Where(c => c.IsAvailable && c.Status == CandidateStatus.Active);
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var candidates = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var matches = candidates.Select(c => new CandidateMatchDto
            {
                CandidateId = c.Id,
                CandidateName = $"{c.FirstName} {c.LastName}",
                CandidateEmail = c.Email,
                MatchScore = 1.0m, // Default score for search results
                MatchingSkills = c.CandidateSkills.Select(cs => cs.Skill.Name).ToList(),
                MissingSkills = new List<string>(),
                Notes = $"Found {c.CandidateSkills.Count} skills"
            }).ToList();

            return matches;
        }

        private decimal CalculateMatchScore(int matchingSkillsCount, int requiredSkillsCount, int candidateSkillsCount)
        {
            if (requiredSkillsCount == 0)
                return 0;

            // Base score: percentage of required skills matched
            var baseScore = (decimal)matchingSkillsCount / requiredSkillsCount;

            // Bonus for having additional relevant skills (up to 20% bonus)
            var bonusFactor = Math.Min(0.2m, (decimal)(candidateSkillsCount - matchingSkillsCount) / (requiredSkillsCount * 5));

            return Math.Min(1.0m, baseScore + bonusFactor);
        }
    }
}
