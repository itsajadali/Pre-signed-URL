namespace StorageService.Services;

public class FileStorageService(IConfiguration configuration) : IFileStorageService
{
    private readonly string uploadPath = configuration["Storage:UploadPath"]
        ?? throw new InvalidOperationException("Upload path not configured.");

    public bool FileExists(string fileId) => Directory.EnumerateFiles(uploadPath, $"{fileId}.*").Any();


    public async Task<string> SaveFileAsync(IFormFile file, string fileId)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{fileId}{extension}";
        var filePath = Path.Combine(uploadPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return fileId;
    }
}
