using System.Reflection;
using BlazorAuthenticationLearn.Shared.Models;
using BlazorAuthenticationLearn.Shared.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YoutubeDLSharp;

namespace BlazorAuthenticationLearn.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class YouTubeController : ControllerBase
{
    [HttpPost("[action]")]
    [Authorize(Roles = nameof(RoleName.YouTube))]
    public async Task<ActionResult<string>> Process(YouTubeRequest youTubeRequest)
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        const string libraryPath = "Lib";
        const string ffmpegName = "ffmpeg";
        const string youtubeDlName = "youtube-dl";
        const string outputDirectory = "YouTubeDownloaded";

        var youTubeDl = new YoutubeDL
        {
            YoutubeDLPath = Path.Join(workingDirectory, libraryPath, youtubeDlName),
            FFmpegPath = Path.Join(workingDirectory, libraryPath, ffmpegName),
            OutputFolder = Path.Join(workingDirectory, outputDirectory)
        };

        var download = await youTubeDl.RunVideoDownload(youTubeRequest.Url);
        var path = download.Data;

        var timeNow = DateTime.Now.Ticks;
        var newFileName = $"{timeNow}.mp4";
        
        System.IO.File.Move(Path.Join(workingDirectory, outputDirectory, Path.GetFileName(path)), 
            Path.Join(workingDirectory, outputDirectory, newFileName));

        return Ok(timeNow.ToString());
    }

    [HttpGet("[action]")]
    [Authorize(Roles = nameof(RoleName.YouTube))]
    public async Task<IActionResult> Download(string filename)
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        const string outputDirectory = "YouTubeDownloaded";
        filename = $"{filename}.mp4";
        var filePath = Path.Join(workingDirectory, outputDirectory, filename);
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", filename);
    }
}
