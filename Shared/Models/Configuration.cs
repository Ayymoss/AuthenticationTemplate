namespace BlazorAuthenticationLearn.Shared.Models;

public class Configuration
{
    public string DataDirectory { get; set; } = @"D:\Users\Amos\RiderProjects\_LearnBlazor\_DataDirectory";
    public string DbConnectionStringName { get; set; } = "BAL_ConnectionString";
    public string DbConnectionString { get; set; } = Environment.GetEnvironmentVariable("BAL_ConnectionString", EnvironmentVariableTarget.User);
    public string DataKeyName { get; set; } = "BAL_EncryptionDecryptionKey";
    public string DataKey { get; set; } = Environment.GetEnvironmentVariable("BAL_EncryptionDecryptionKey", EnvironmentVariableTarget.User);
}
