using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Model.DTO;
using Server.Model.Entities;
using System.Data;
using System.Text.RegularExpressions;

namespace Server.Services
{
    public class ExcelImportService : IExcelImportService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Candidate> _candidateRepository;
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<CandidateSkill> _candidateSkillRepository;

        public ExcelImportService(
            AppDbContext context,
            IRepository<Candidate> candidateRepository,
            IRepository<Skill> skillRepository,
            IRepository<CandidateSkill> candidateSkillRepository)
        {
            _context = context;
            _candidateRepository = candidateRepository;
            _skillRepository = skillRepository;
            _candidateSkillRepository = candidateSkillRepository;
        }

        public async Task<BulkImportResultDto> ImportCandidatesFromExcelAsync(ExcelImportDto importDto, int uploadedByUserId)
        {
            var result = new BulkImportResultDto();

            try
            {
                // Validate file
                if (importDto.File == null || importDto.File.Length == 0)
                {
                    result.Success = false;
                    result.Message = "No file provided";
                    return result;
                }

                var fileExtension = Path.GetExtension(importDto.File.FileName).ToLowerInvariant();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    result.Success = false;
                    result.Message = "Invalid file type. Only Excel files (.xlsx, .xls) are allowed.";
                    return result;
                }

                // Read Excel file
                var candidates = await ReadExcelFileAsync(importDto.File);
                result.TotalRows = candidates.Count;

                var createdCandidates = new List<CandidateDto>();
                var errors = new List<ImportErrorDto>();

                foreach (var candidateData in candidates)
                {
                    try
                    {
                        // Validate candidate data
                        var validationErrors = await ValidateCandidateDataAsync(candidateData, candidateData.RowNumber);
                        if (validationErrors.Any())
                        {
                            errors.AddRange(validationErrors.Select(error => new ImportErrorDto
                            {
                                RowNumber = candidateData.RowNumber,
                                FieldName = "General",
                                ErrorMessage = error,
                                RowData = $"{candidateData.FirstName} {candidateData.LastName} ({candidateData.Email})"
                            }));
                            continue;
                        }

                        // Check for duplicates if SkipDuplicates is enabled
                        if (importDto.SkipDuplicates)
                        {
                            var existingCandidate = await _context.Candidates
                                .FirstOrDefaultAsync(c => c.Email == candidateData.Email);
                            if (existingCandidate != null)
                            {
                                errors.Add(new ImportErrorDto
                                {
                                    RowNumber = candidateData.RowNumber,
                                    FieldName = "Email",
                                    ErrorMessage = "Candidate with this email already exists",
                                    RowData = candidateData.Email
                                });
                                continue;
                            }
                        }

                        // Create candidate
                        var candidate = new Candidate
                        {
                            FirstName = candidateData.FirstName,
                            LastName = candidateData.LastName,
                            Email = candidateData.Email,
                            PhoneNumber = candidateData.PhoneNumber,
                            Address = candidateData.Address,
                            City = candidateData.City,
                            State = candidateData.State,
                            Country = candidateData.Country,
                            PostalCode = candidateData.PostalCode,
                            DateOfBirth = candidateData.DateOfBirth,
                            CurrentPosition = candidateData.CurrentPosition,
                            CurrentCompany = candidateData.CurrentCompany,
                            CurrentSalary = candidateData.CurrentSalary,
                            ExpectedSalary = candidateData.ExpectedSalary,
                            ExperienceSummary = candidateData.ExperienceSummary,
                            Education = candidateData.Education,
                            Certifications = candidateData.Certifications,
                            Languages = candidateData.Languages,
                            Notes = candidateData.Notes,
                            LinkedInProfile = candidateData.LinkedInProfile,
                            PortfolioUrl = candidateData.PortfolioUrl,
                            Source = CandidateSource.ExcelImport,
                            SourceDetails = importDto.SourceDetails ?? $"Excel Import: {importDto.File.FileName}",
                            CreatedByUserId = uploadedByUserId,
                            Status = CandidateStatus.Active,
                            IsAvailable = candidateData.IsAvailable,
                            AvailableFromDate = candidateData.AvailableFromDate
                        };

                        await _candidateRepository.AddAsync(candidate);
                        await _candidateRepository.SaveChangesAsync();

                        // Add skills if provided
                        if (candidateData.CandidateSkills.Any())
                        {
                            await AddSkillsToCandidateAsync(candidate.Id, candidateData.CandidateSkills, importDto.AutoCreateSkills);
                        }

                        // Add to successful imports
                        result.SuccessfulImports++;

                        // Get the created candidate with full details for response
                        var createdCandidate = await _context.Candidates
                            .Include(c => c.CreatedByUser)
                            .Include(c => c.CandidateSkills)
                                .ThenInclude(cs => cs.Skill)
                            .FirstOrDefaultAsync(c => c.Id == candidate.Id);

                        if (createdCandidate != null)
                        {
                            createdCandidates.Add(MapToCandidateDto(createdCandidate));
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new ImportErrorDto
                        {
                            RowNumber = candidateData.RowNumber,
                            FieldName = "General",
                            ErrorMessage = $"Error creating candidate: {ex.Message}",
                            RowData = $"{candidateData.FirstName} {candidateData.LastName} ({candidateData.Email})"
                        });
                    }
                }

                result.FailedImports = errors.Count;
                result.Errors = errors;
                result.CreatedCandidates = createdCandidates;

                if (result.SuccessfulImports > 0)
                {
                    result.Success = true;
                    result.Message = $"Successfully imported {result.SuccessfulImports} candidates. {result.FailedImports} failed.";
                }
                else
                {
                    result.Success = false;
                    result.Message = "No candidates were imported successfully.";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error processing Excel file: {ex.Message}";
                result.Errors.Add(new ImportErrorDto
                {
                    RowNumber = 0,
                    FieldName = "File",
                    ErrorMessage = ex.Message,
                    RowData = importDto.File.FileName
                });
            }

            return result;
        }

        public async Task<byte[]> GenerateExcelTemplateAsync()
        {
            var template = new StringBuilder();
            template.AppendLine("FirstName,LastName,Email,PhoneNumber,Address,City,State,Country,PostalCode,DateOfBirth,CurrentPosition,CurrentCompany,CurrentSalary,ExpectedSalary,ExperienceSummary,Education,Certifications,Languages,Notes,LinkedInProfile,PortfolioUrl,IsAvailable,AvailableFromDate,Skills");
            template.AppendLine("John,Doe,john.doe@email.com,+1234567890,123 Main St,New York,NY,USA,10001,1990-01-01,Software Developer,Tech Corp,75000,85000,5 years of experience,Computer Science Degree,AWS Certified,English;Spanish,Excellent candidate,https://linkedin.com/in/johndoe,https://portfolio.com/johndoe,true,2024-01-01,JavaScript;C#;SQL;React");

            return Encoding.UTF8.GetBytes(template.ToString());
        }

        public async Task<List<string>> ValidateCandidateDataAsync(CreateCandidateDto candidate, int rowNumber)
        {
            var errors = new List<string>();

            // Required fields validation
            if (string.IsNullOrWhiteSpace(candidate.FirstName))
                errors.Add($"Row {rowNumber}: First Name is required");

            if (string.IsNullOrWhiteSpace(candidate.LastName))
                errors.Add($"Row {rowNumber}: Last Name is required");

            if (string.IsNullOrWhiteSpace(candidate.Email))
                errors.Add($"Row {rowNumber}: Email is required");
            else if (!IsValidEmail(candidate.Email))
                errors.Add($"Row {rowNumber}: Invalid email format");

            // Optional field validations
            if (!string.IsNullOrWhiteSpace(candidate.PhoneNumber) && !IsValidPhoneNumber(candidate.PhoneNumber))
                errors.Add($"Row {rowNumber}: Invalid phone number format");

            if (candidate.DateOfBirth.HasValue && candidate.DateOfBirth.Value > DateTime.Now)
                errors.Add($"Row {rowNumber}: Date of Birth cannot be in the future");

            if (candidate.CurrentSalary.HasValue && candidate.CurrentSalary.Value < 0)
                errors.Add($"Row {rowNumber}: Current Salary cannot be negative");

            if (candidate.ExpectedSalary.HasValue && candidate.ExpectedSalary.Value < 0)
                errors.Add($"Row {rowNumber}: Expected Salary cannot be negative");

            return errors;
        }

        private async Task<List<CreateCandidateDto>> ReadExcelFileAsync(IFormFile file)
        {
            var candidates = new List<CreateCandidateDto>();

            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                if (lines.Count < 2)
                    return candidates;

                var headers = lines[0].Split(',');
                var headerMap = CreateHeaderMap(headers);

                for (int i = 1; i < lines.Count; i++)
                {
                    try
                    {
                        var values = ParseCsvLine(lines[i]);
                        if (values.Length < headers.Length)
                            continue;

                        var candidate = new CreateCandidateDto
                        {
                            RowNumber = i + 1,
                            FirstName = GetValue(values, headerMap, "FirstName"),
                            LastName = GetValue(values, headerMap, "LastName"),
                            Email = GetValue(values, headerMap, "Email"),
                            PhoneNumber = GetValue(values, headerMap, "PhoneNumber"),
                            Address = GetValue(values, headerMap, "Address"),
                            City = GetValue(values, headerMap, "City"),
                            State = GetValue(values, headerMap, "State"),
                            Country = GetValue(values, headerMap, "Country"),
                            PostalCode = GetValue(values, headerMap, "PostalCode"),
                            CurrentPosition = GetValue(values, headerMap, "CurrentPosition"),
                            CurrentCompany = GetValue(values, headerMap, "CurrentCompany"),
                            ExperienceSummary = GetValue(values, headerMap, "ExperienceSummary"),
                            Education = GetValue(values, headerMap, "Education"),
                            Certifications = GetValue(values, headerMap, "Certifications"),
                            Languages = GetValue(values, headerMap, "Languages"),
                            Notes = GetValue(values, headerMap, "Notes"),
                            LinkedInProfile = GetValue(values, headerMap, "LinkedInProfile"),
                            PortfolioUrl = GetValue(values, headerMap, "PortfolioUrl"),
                            IsAvailable = GetBoolValue(values, headerMap, "IsAvailable", true),
                            CandidateSkills = new List<CreateCandidateSkillDto>()
                        };

                        // Parse date fields
                        var dateOfBirthStr = GetValue(values, headerMap, "DateOfBirth");
                        if (DateTime.TryParse(dateOfBirthStr, out DateTime dateOfBirth))
                            candidate.DateOfBirth = dateOfBirth;

                        var availableFromDateStr = GetValue(values, headerMap, "AvailableFromDate");
                        if (DateTime.TryParse(availableFromDateStr, out DateTime availableFromDate))
                            candidate.AvailableFromDate = availableFromDate;

                        // Parse salary fields
                        var currentSalaryStr = GetValue(values, headerMap, "CurrentSalary");
                        if (decimal.TryParse(currentSalaryStr, out decimal currentSalary))
                            candidate.CurrentSalary = currentSalary;

                        var expectedSalaryStr = GetValue(values, headerMap, "ExpectedSalary");
                        if (decimal.TryParse(expectedSalaryStr, out decimal expectedSalary))
                            candidate.ExpectedSalary = expectedSalary;

                        // Parse skills
                        var skillsStr = GetValue(values, headerMap, "Skills");
                        if (!string.IsNullOrWhiteSpace(skillsStr))
                        {
                            var skillNames = skillsStr.Split(';', ',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var skillName in skillNames.Take(10)) // Limit to 10 skills per candidate
                            {
                                candidate.CandidateSkills.Add(new CreateCandidateSkillDto
                                {
                                    SkillName = skillName.Trim(),
                                    Level = 2, // Default to Intermediate
                                    YearsOfExperience = 1,
                                    IsVerified = false,
                                    Notes = "Imported from Excel"
                                });
                            }
                        }

                        candidates.Add(candidate);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                // Return empty list on error
            }

            return candidates;
        }

        private Dictionary<string, int> CreateHeaderMap(string[] headers)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < headers.Length; i++)
            {
                map[headers[i].Trim()] = i;
            }
            return map;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            result.Add(current.ToString().Trim());
            return result.ToArray();
        }

        private string GetValue(string[] values, Dictionary<string, int> headerMap, string fieldName)
        {
            if (headerMap.TryGetValue(fieldName, out int index) && index < values.Length)
            {
                return values[index].Trim().Trim('"');
            }
            return "";
        }

        private bool GetBoolValue(string[] values, Dictionary<string, int> headerMap, string fieldName, bool defaultValue)
        {
            var value = GetValue(values, headerMap, fieldName);
            if (bool.TryParse(value, out bool result))
                return result;
            return defaultValue;
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        private bool IsValidPhoneNumber(string phone)
        {
            var phonePattern = @"^[\+]?[\d\s\-\(\)]{10,}$";
            return Regex.IsMatch(phone, phonePattern);
        }

        private async Task AddSkillsToCandidateAsync(int candidateId, List<CreateCandidateSkillDto> skillDtos, bool autoCreateSkills)
        {
            foreach (var skillDto in skillDtos)
            {
                try
                {
                    Skill skill;

                    if (!string.IsNullOrWhiteSpace(skillDto.SkillName))
                    {
                        skill = await _context.Skills
                            .FirstOrDefaultAsync(s => s.Name.ToLower() == skillDto.SkillName.ToLower());

                        if (skill == null && autoCreateSkills)
                        {
                            // Create new skill
                            skill = new Skill
                            {
                                Name = skillDto.SkillName,
                                Description = $"Auto-generated skill from Excel import",
                                SkillCategoryId = 1, // Default category
                                IsActive = true
                            };
                            await _skillRepository.AddAsync(skill);
                            await _skillRepository.SaveChangesAsync();
                        }
                    }
                    else if (skillDto.SkillId > 0)
                    {
                        // Find skill by ID
                        skill = await _skillRepository.FindByIdAsync(skillDto.SkillId);
                    }
                    else
                    {
                        continue; // Skip if no valid skill identifier
                    }

                    if (skill != null)
                    {
                        // Check if candidate already has this skill
                        var existingCandidateSkill = await _context.CandidateSkills
                            .FirstOrDefaultAsync(cs => cs.CandidateId == candidateId && cs.SkillId == skill.Id);

                        if (existingCandidateSkill == null)
                        {
                            // Add skill to candidate
                            var candidateSkill = new CandidateSkill
                            {
                                CandidateId = candidateId,
                                SkillId = skill.Id,
                                Level = (SkillLevel)skillDto.Level,
                                YearsOfExperience = skillDto.YearsOfExperience,
                                Notes = skillDto.Notes,
                                LastUsed = skillDto.LastUsed,
                                IsVerified = skillDto.IsVerified
                            };

                            await _candidateSkillRepository.AddAsync(candidateSkill);
                        }
                    }
                }
                catch (Exception)
                {
                    // Skip this skill if there's an error
                    continue;
                }
            }

            await _candidateSkillRepository.SaveChangesAsync();
        }

        private CandidateDto MapToCandidateDto(Candidate candidate)
        {
            return new CandidateDto
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                Email = candidate.Email,
                PhoneNumber = candidate.PhoneNumber,
                Address = candidate.Address,
                City = candidate.City,
                State = candidate.State,
                Country = candidate.Country,
                PostalCode = candidate.PostalCode,
                DateOfBirth = candidate.DateOfBirth,
                CurrentPosition = candidate.CurrentPosition,
                CurrentCompany = candidate.CurrentCompany,
                CurrentSalary = candidate.CurrentSalary,
                ExpectedSalary = candidate.ExpectedSalary,
                ExperienceSummary = candidate.ExperienceSummary,
                Education = candidate.Education,
                Certifications = candidate.Certifications,
                Languages = candidate.Languages,
                Notes = candidate.Notes,
                LinkedInProfile = candidate.LinkedInProfile,
                PortfolioUrl = candidate.PortfolioUrl,
                Status = (int)candidate.Status,
                StatusName = candidate.Status.ToString(),
                Source = (int)candidate.Source,
                SourceName = candidate.Source.ToString(),
                SourceDetails = candidate.SourceDetails,
                CreatedAt = candidate.CreatedAt,
                UpdatedAt = candidate.UpdatedAt,
                CreatedByUserId = candidate.CreatedByUserId,
                CreatedByUserEmail = candidate.CreatedByUser?.Email ?? "",
                LastContactDate = candidate.LastContactDate,
                LastContactNotes = candidate.LastContactNotes,
                IsAvailable = candidate.IsAvailable,
                AvailableFromDate = candidate.AvailableFromDate,
                CandidateSkills = candidate.CandidateSkills.Select(cs => new CandidateSkillDto
                {
                    Id = cs.Id,
                    CandidateId = cs.CandidateId,
                    SkillId = cs.SkillId,
                    SkillName = cs.Skill?.Name ?? "",
                    SkillCategoryName = cs.Skill?.SkillCategory?.Name ?? "",
                    Level = (int)cs.Level,
                    LevelName = cs.Level.ToString(),
                    YearsOfExperience = cs.YearsOfExperience,
                    Notes = cs.Notes,
                    LastUsed = cs.LastUsed,
                    IsVerified = cs.IsVerified,
                    CreatedAt = cs.CreatedAt,
                    UpdatedAt = cs.UpdatedAt
                }).ToList()
            };
        }
    }

    // Helper class for Excel import
    public class CreateCandidateDto
    {
        public int RowNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? CurrentPosition { get; set; }
        public string? CurrentCompany { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? ExperienceSummary { get; set; }
        public string? Education { get; set; }
        public string? Certifications { get; set; }
        public string? Languages { get; set; }
        public string? Notes { get; set; }
        public string? LinkedInProfile { get; set; }
        public string? PortfolioUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? AvailableFromDate { get; set; }
        public List<CreateCandidateSkillDto> CandidateSkills { get; set; } = new List<CreateCandidateSkillDto>();
    }
}
