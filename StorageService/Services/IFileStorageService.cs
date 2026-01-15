namespace StorageService.Services;

public interface IFileStorageService
{
    Task<bool> FileExistsAsync(string fileId, CancellationToken cancellationToken = default);
    Task<string> SaveFileAsync(IFormFile file, string fileId, CancellationToken cancellationToken = default);
}
