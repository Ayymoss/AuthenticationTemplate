using System.Net;
using BlazorAuthenticationLearn.Server.Utilities;
using BlazorAuthenticationLearn.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text;
using BlazorAuthenticationLearn.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly PostgresqlDataContext _context;

    public const long MaxFileSize = 50_000_000;

    public FileController(ILogger<FileController> logger, PostgresqlDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpPost("Upload"), Authorize, RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<IList<UploadResult>>> UploadFile([FromForm] IEnumerable<IFormFile> files)
    {
        const int maxAllowedFiles = 50;
        var filesProcessed = 0;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        var uploadResults = new List<UploadResult>();
        var encDec = new EncryptionDecryption();

        foreach (var file in files)
        {
            var uploadResult = new UploadResult();
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

            if (filesProcessed < maxAllowedFiles)
            {
                switch (file.Length)
                {
                    case 0:
                        _logger.LogInformation("{FileName} length is 0 (Err: 1)", trustedFileNameForDisplay);
                        uploadResult.ErrorCode = 1;
                        break;
                    case > MaxFileSize:
                        _logger.LogInformation(
                            "{FileName} of {Length} bytes is larger than the limit of {Limit} bytes (Err: 2)",
                            trustedFileNameForDisplay, file.Length, MaxFileSize);
                        uploadResult.ErrorCode = 2;
                        break;
                    default:
                        try
                        {
                            // TODO: Returned bytearray on ln 61 from encryption is not the correct length as what you'd expect
                            var byteArray = new byte[file.Length];
                            var readAsync = await file.OpenReadStream()
                                .ReadAsync(byteArray.AsMemory(0, (int) file.Length));
                            var (encryptedArray, iv) =
                                encDec.EncryptString(Environment.GetEnvironmentVariable("EncryptionDecryptionKey"),
                                    byteArray);
                            
                            

                            uploadResult.Uploaded = true;
                            uploadResult.StoredFileName = file.FileName;

                            var fileToStore = new FileContext
                            {
                                Name = file.FileName,
                                Size = file.Length,
                                Iv = iv,
                                UploadDate = DateTimeOffset.UtcNow,
                                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                                UserName = User.Identity.Name,
                                Data = encryptedArray
                            };

                            _context.Add(fileToStore);
                            await _context.SaveChangesAsync();
                            
                            await file.OpenReadStream().DisposeAsync();
                        }
                        catch (IOException ex)
                        {
                            _logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                                trustedFileNameForDisplay, ex.Message);
                            uploadResult.ErrorCode = 3;
                        }

                        break;
                }

                filesProcessed++;
            }
            else
            {
                _logger.LogInformation(
                    "{FileName} not uploaded because the " + "request exceeded the allowed {Count} of files (Err: 4)",
                    trustedFileNameForDisplay, maxAllowedFiles);
                uploadResult.ErrorCode = 4;
            }

            uploadResults.Add(uploadResult);
        }

        return new CreatedResult(resourcePath, uploadResults);
    }

    [HttpGet("GetFiles"), Authorize]
    public async Task<ActionResult<IList<FileContext>>> GetFiles()
    {
        var files = await _context.FileContexts.AsNoTracking()
            .Select(x => new {x.Id, x.Name, x.Size, x.UploadDate, x.UserId, x.UserName}).ToListAsync();
        return Ok(files);
    }

    [HttpGet("Download/{id:int}"), Authorize]
    public async Task<ActionResult> DownloadFile(int id)
    {
        var file = await _context.FileContexts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (file == null)
        {
            return NotFound();
        }

        var decryptedFile =
            new EncryptionDecryption().DecryptString(Environment.GetEnvironmentVariable("EncryptionDecryptionKey"),
                file.Data, file.Iv);

        return File(decryptedFile, "application/octet-stream", file.Name);
    }


    [HttpDelete("Delete/{id:int}"), Authorize]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var fileMeta = new FileContext {Id = id};
        _context.Remove(fileMeta);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
