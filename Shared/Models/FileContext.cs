namespace BlazorAuthenticationLearn.Shared.Models;

public class FileContext
{
    public int Id { get; set; }
    public string UploaderId { get; set; }
    public string UploaderName { get; set; }
    public string FileName { get; set; }
    public string FileNameGuid { get; set; }
    public long FileSize { get; set; }
    public byte[] FileIv { get; set; }
    public DateTimeOffset UploadDate { get; set; }
}
