using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AppService.Microservices.StorageService;

public record GeneratePreSignedUrlRequest(string id,
                                          string FileName,
                                          string ContentType,
                                          long FileSizeLimit,
                                          string signature,
                                          long expiresAt);

public record GeneratePreSignedUrlResponse(string SignedUrl);


public class StorgeService(IHttpClientFactory factoy,
                           JsonSerializerOptions jsonSerializerOptions,
                           ILogger<StorgeService> logger) : IStorageService
{
    public async Task<bool> CheckFileExistanceAsync(string fileId)
    {
        logger.LogInformation("Checking file existence for FileId: {FileId}", fileId);

        try
        {
            var httpClient = factoy.CreateClient("StorageService");

            var request = new HttpRequestMessage(HttpMethod.Head, $"api/Files/{fileId}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            var exists = response.StatusCode switch
            {
                HttpStatusCode.OK => true,
                HttpStatusCode.NotFound => false,
                _ => throw new HttpRequestException($"Unexpected status code: {response.StatusCode}")
            };

            if (exists)
            {
                logger.LogInformation("File exists for FileId: {FileId}", fileId);
            }
            else
            {
                logger.LogInformation("File not found for FileId: {FileId}", fileId);
            }

            return exists;
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking file existence for FileId: {FileId}", fileId);
            throw;
        }
    }

    public async Task<GeneratePreSignedUrlResponse> GeneratePreSignedUrlAsync(GeneratePreSignedUrlRequest requestBody,
                                                                              CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Generating pre-signed URL for FileId: {FileId}", requestBody.id);

        try
        {
            var httpClient = factoy.CreateClient("StorageService");

            using var memoryContentStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(memoryContentStream, requestBody, cancellationToken: cancellationToken);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using var request = new HttpRequestMessage(HttpMethod.Post,
                                                       "api/Files/GenratePreSignedUrl");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var streamContent = new StreamContent(memoryContentStream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = streamContent;

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError("Failed to generate pre-signed URL. Status Code: {StatusCode}", response.StatusCode);
            }

            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var signedUrl = await JsonSerializer.DeserializeAsync<GeneratePreSignedUrlResponse>(stream,
                                                                                                jsonSerializerOptions,
                                                                                                cancellationToken);

            if (signedUrl == null)
            {
                logger.LogError("Failed to deserialize pre-signed URL response");
                throw new InvalidOperationException("Failed to deserialize pre-signed URL response");
            }

            logger.LogInformation("Successfully generated pre-signed URL for FileId: {FileId}", requestBody.id);

            return signedUrl;
        }


        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating pre-signed URL for FileId: {FileId}", requestBody.id);
            throw;
        }
    }
}

