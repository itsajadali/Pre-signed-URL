using Common.Secuirty;
using Microsoft.AspNetCore.Mvc;
using StorageService.Model;
using StorageService.Services;
using StorageService.Utilities;


namespace StorageService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController(IFileStorageService storageService,
                             ILogger<FilesController> logger,
                             IConfiguration configuration) : ControllerBase
{

    private readonly string secretKey = configuration["Signing:Secret"]!;


    [HttpHead("{fileId}")]
    public ActionResult GetFile(string fileId)
    {
        if (storageService.FileExists(fileId))
            return Ok();

        return NotFound();
    }

    [HttpPost("UploadFile")]
    public async Task<IActionResult> UploadFile([FromQuery] FileMetadata metadata, IFormFile file)
    {
        var dict = new Dictionary<string, string>
        {
            { "Id", metadata.Id },
            { "FileName", file.FileName },
            { "ContentType", file.ContentType },
            { "FileSizeLimit", file.Length.ToString() },
            { "ExpiresAt", metadata.ExpiresAt.ToString() }
        };


        if (!Secuirty.Verify(dict, metadata.Signature, metadata.ExpiresAt, secretKey))
            return BadRequest("Invalid or expired signature.");


        if (storageService.FileExists(metadata.Id))
            return Conflict(new { message = $"A file with ID '{metadata.Id}' already exists." });


        await storageService.SaveFileAsync(file, metadata.Id);

        logger.LogInformation("File {Id} uploaded successfully", metadata.Id);

        return Ok(new
        {
            imageId = metadata.Id
        });
    }

    [HttpPost("GenratePreSignedUrl")]
    public IActionResult GenratePreSignedUrl(FileMetadata metadata)
    {
        logger.LogInformation("Generate pre-signed URL request for FileId: {FileId}, FileName: {FileName}", metadata.Id, metadata.FileName);

        var isSingrueVailed = Secuirty.Verify(new Dictionary<string, string>
                              {
                                { "Id", metadata.Id },
                                { "FileName", metadata.FileName },
                                { "ContentType", metadata.ContentType },
                                { "FileSizeLimit", metadata.FileSizeLimit.ToString()},
                                { "ExpiresAt", metadata.ExpiresAt.ToString() },
                              }, metadata.Signature, metadata.ExpiresAt, secretKey);

        if (!isSingrueVailed)
            return BadRequest("Invalid request: Signature expired or tampered with.");


        var uniqueId = Guid.NewGuid().ToString();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();

        var metaDataToSign = new Dictionary<string, string>
        {
            { "Id", uniqueId },
            { "FileName", metadata.FileName },
            { "ContentType", metadata.ContentType },
            { "FileSizeLimit", metadata.FileSizeLimit.ToString() }
        };

        var signature = Secuirty.Sign(metaDataToSign, expiresAt, secretKey);

        var publicBaseUrl = configuration["StorageService:PublicBaseUrl"];

        var baseUrl = $"{publicBaseUrl}/api/Files/UploadFile";
        var queryString = metaDataToSign.BuildQueryString() + $"&signature={signature}&expiresAt={expiresAt}";

        return Ok(new { signedUrl = $"{baseUrl}{queryString}" });
    }
}
