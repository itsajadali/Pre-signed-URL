using Microsoft.Extensions.Caching.Distributed;

namespace StorageService.Services;

public class FileStorageService(IConfiguration configuration,
                                IDistributedCache cache) : IFileStorageService
{
    private readonly string uploadPath = configuration["Storage:UploadPath"]
        ?? throw new InvalidOperationException("Upload path not configured.");

    public async Task<bool> FileExistsAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"file:{fileId}";
        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is not null)
            return true;

        var exists = Directory.EnumerateFiles(uploadPath, $"{fileId}.*").Any();

        await cache.SetStringAsync(
            cacheKey,
            exists.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return exists;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string fileId, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{fileId}{extension}";
        var filePath = Path.Combine(uploadPath, fileName);

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        var fileBytes = memoryStream.ToArray();

        await File.WriteAllBytesAsync(filePath, fileBytes, cancellationToken);

        var cacheKey = $"file:{fileId}";

        Task task = cache.SetAsync(cacheKey,
                                   fileBytes,
                                   new DistributedCacheEntryOptions
                                   {
                                       AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                                   },
                                   cancellationToken);

        return fileId;
    }
}
