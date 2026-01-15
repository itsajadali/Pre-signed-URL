using AppService.Microservices.StorageService;
using AppService.Models;
using Common.Secuirty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Seller")]
public class FilesController(IConfiguration configuration,
                             IStorageService storgeService,
                             ILogger<FilesController> logger) : ControllerBase
{
    private readonly string secretKey = configuration["Signing:Secret"]!;


    [HttpPost("RequestUploadUrl")]
    public async Task<IActionResult> RequestUploadUrl(RequestUploadUrlDto requestUploadUrl)
    {
        logger.LogInformation("Requesting upload url for file {FileName}", requestUploadUrl.FileName);

        var id = Guid.NewGuid().ToString("");

        Dictionary<string, string> metaDataToSign = new()
        {
            { "Id", id },
            { "FileName", requestUploadUrl.FileName },
            { "ContentType", requestUploadUrl.ContentType },
            { "FileSizeLimit", requestUploadUrl.FileSizeLimit.ToString() },
        };

        var expiration = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
        string signature = Secuirty.Sign(metaDataToSign, expiration, secretKey);

        var presignedUrl = await storgeService.GeneratePreSignedUrlAsync(new(id,
                                                                             FileName: requestUploadUrl.FileName,
                                                                             ContentType: requestUploadUrl.ContentType,
                                                                             FileSizeLimit: requestUploadUrl.FileSizeLimit,
                                                                             signature,
                                                                             expiration));

        return Ok(presignedUrl);
    }

}

