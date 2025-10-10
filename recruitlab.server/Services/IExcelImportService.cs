using Server.Model.DTO;

namespace Server.Services
{
    public interface IExcelImportService
    {
        Task<BulkImportResultDto> ImportCandidatesFromExcelAsync(ExcelImportDto importDto, int uploadedByUserId);
        Task<byte[]> GenerateExcelTemplateAsync();
        Task<List<string>> ValidateCandidateDataAsync(CreateCandidateDto candidate, int rowNumber);
    }
}
