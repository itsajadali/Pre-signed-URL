namespace StorageService.Services;

public interface IFileStorageService
{
    bool FileExists(string fileId);
    Task<string> SaveFileAsync(IFormFile file, string fileId);
}
