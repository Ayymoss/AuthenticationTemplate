using BlazorAuthenticationLearn.Shared.Models;
using BlazorAuthenticationLearn.Shared.Utilities;

namespace BlazorAuthenticationLearn.Server.Utilities;

public class DatabaseConfigurationProperties
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
}

public static class DatabaseConfiguration
{
    internal static DatabaseConfigurationProperties PromptBuildConfig()
    {
        var dbCfgProps = new DatabaseConfigurationProperties();
        var confirm = false;


        while (confirm == false)
        {
            dbCfgProps.HostName = "Host".PromptString();
            dbCfgProps.Port = "Port".PromptInt(defaultValue: 5432);
            dbCfgProps.UserName = "Username".PromptString();
            dbCfgProps.Password = "Password".PromptString();
            dbCfgProps.Database = "Database".PromptString();
            confirm = "Information Correct".PromptBool(defaultValue: true);
        }

        return dbCfgProps;
    }
}
