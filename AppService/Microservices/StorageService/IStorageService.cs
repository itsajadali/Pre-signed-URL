namespace AppService.Microservices.StorageService;

public interface IStorageService
{
    Task<bool> CheckFileExistanceAsync(string fileId);
    Task<GeneratePreSignedUrlResponse> GeneratePreSignedUrlAsync(GeneratePreSignedUrlRequest requestBody,
                                                                 CancellationToken cancellationToken = default);
}

