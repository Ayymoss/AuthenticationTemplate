using BlazorAuthenticationLearn.Shared.Utilities;

namespace BlazorAuthenticationLearn.Server;

public static class DatabaseConfiguration
{
    internal static void DatabaseInit()
    {
        var confirm = false;
        var host = string.Empty;
        var port = 0;
        var database = string.Empty;
        var username = string.Empty;
        var password = string.Empty;

        while (confirm == false)
        {
            host = "Host".PromptString();
            port = "Port".PromptInt(defaultValue: 5432);
            username = "Username".PromptString();
            password = "Password".PromptString();
            database = "Database".PromptString();
            confirm = "Information Correct".PromptBool(defaultValue: false);
        }

        var connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
        Environment.SetEnvironmentVariable("BAL_ConnectionString", connectionString, EnvironmentVariableTarget.User);
    }
}
