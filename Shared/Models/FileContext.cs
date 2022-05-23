namespace BlazorAuthenticationLearn.Shared.Models;

public class FileContext
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public DateTimeOffset UploadDate { get; set; }
    public long Size { get; set; }
    public byte[] Iv { get; set; }
    public byte[] Data { get; set; }
}
