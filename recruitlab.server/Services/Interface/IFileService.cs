namespace recruitlab.server.Services.Interface
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string subDirectory);
        void DeleteFile(string filePath);
    }
}
