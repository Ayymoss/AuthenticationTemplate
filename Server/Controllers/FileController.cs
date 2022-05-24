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
    public async Task<ActionResult<bool>> UploadFile([FromBody] FileChunk fileChunk)
    {
        try
        {
            // get the local filename
            var filePath = Path.Join(Environment.CurrentDirectory + "Files");
            var fileName = filePath + fileChunk.FileNameNoPath;

            // delete the file if necessary
            if (fileChunk.FirstChunk && System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            // open for writing
            await using var stream = System.IO.File.OpenWrite(fileName);
            stream.Seek(fileChunk.Offset, SeekOrigin.Begin);
            stream.Write(fileChunk.Data, 0, fileChunk.Data.Length);

            return true;
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            return false;
        }
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
