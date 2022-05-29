using BlazorAuthenticationLearn.Server.Utilities;
using BlazorAuthenticationLearn.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlazorAuthenticationLearn.Server.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly PostgresqlDataContext _context;
    private readonly Configuration _configuration;

    public FileController(PostgresqlDataContext context, Configuration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("[action]"), Authorize]
    public async Task<ActionResult<IList<FileContext>>> GetFiles()
    {
        var files = await _context.FileContexts.AsNoTracking()
            .Select(x => new {x.FileNameGuid, x.FileName, x.FileSize, x.UploadDate, x.UploaderName})
            .Where(x => x.UploaderName == User.Identity.Name).ToListAsync();
        return Ok(files);
    }

    [HttpGet("[action]/{guid}"), Authorize]
    public async Task<IActionResult> Download(string guid)
    {
        var file = await _context.FileContexts.AsNoTracking().FirstOrDefaultAsync(x => x.FileNameGuid == guid);
        if (file == null)
        {
            return NotFound();
        }
        if (file.UploaderName != User.Identity!.Name)
        {
            return Unauthorized();
        }

        var filename = Path.Join(_configuration.DataDirectory, User.Identity!.Name, file.FileNameGuid);
        await using var fs = System.IO.File.OpenRead(filename);
        var ms = new MemoryStream();
        EncryptionDecryption.Decrypt(_configuration.DataKey, file.FileIv, fs, ms);
        ms.Seek(0, SeekOrigin.Begin);
        return File(ms, "application/octet-stream", file.FileName);
    }


    [HttpDelete("[action]/{guid}"), Authorize]
    public async Task<IActionResult> Delete(string guid)
    {
        var file = await _context.FileContexts.AsNoTracking().FirstOrDefaultAsync(x => x.FileNameGuid == guid);
        if (file == null)
        {
            return NotFound();
        }

        System.IO.File.Delete(Path.Join(_configuration.DataDirectory, User.Identity!.Name, file.FileNameGuid));
        _context.Remove(file);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("[action]"), RequestSizeLimit(100_000_000), Authorize]
    public async Task<IActionResult> Upload(IList<IFormFile> uploadFiles)
    {
        Directory.CreateDirectory(Path.Join(_configuration.DataDirectory, User.Identity!.Name));

        try
        {
            foreach (var file in uploadFiles)
            {
                // File length check doesn't actually display the error to the user. Can be a bit confusing.
                if (file.FileName.Length > 128)
                {
                    return BadRequest("File name is too long");
                }
                
                var fileGuid = Guid.NewGuid().ToString();
                var filename = Path.Join(_configuration.DataDirectory, User.Identity!.Name, fileGuid);

                if (System.IO.File.Exists(filename)) continue;

                await using var fs = System.IO.File.Create(filename);
                var sourceStream = file.OpenReadStream();
                var iv = EncryptionDecryption.Encrypt(_configuration.DataKey, sourceStream, fs);

                var fileToStore = new FileContext
                {
                    UploaderId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    UploaderName = User.Identity.Name,
                    FileName = file.FileName,
                    FileNameGuid = fileGuid,
                    FileSize = file.Length,
                    FileIv = iv,
                    UploadDate = DateTimeOffset.UtcNow,
                };

                _context.Add(fileToStore);
                await _context.SaveChangesAsync();

                fs.Flush();
                await sourceStream.DisposeAsync();
            }
        }
        catch (Exception e)
        {
            Response.Clear();
            Response.StatusCode = 204;
            Response.HttpContext.Features.Get<IHttpResponseFeature>()!.ReasonPhrase = "File failed to upload";
            Response.HttpContext.Features.Get<IHttpResponseFeature>()!.ReasonPhrase = e.Message;
        }

        return NoContent();
    }

    //https://blazor.syncfusion.com/documentation/file-upload/how-to/getting-started-with-blazor-webassembly
}
