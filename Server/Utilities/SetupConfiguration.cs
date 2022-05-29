using System.Reflection;
using System.Text;
using System.Text.Json;

namespace BlazorAuthenticationLearn.Server.Utilities;

public class Configuration
{
    public byte Version { get; set; } = 1;
    public string DataDirectory { get; set; }
    public string DataKey { get; set; }
    public DatabaseConfigurationProperties Database { get; set; }
}

public static class SetupConfiguration
{
    public static async void InitConfiguration()
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (File.Exists(Path.Join(workingDirectory, "GlobalConfiguration.json"))) return;

        var dbConfig = DatabaseConfiguration.PromptBuildConfig();
        var random = new Random();
        var dataKey = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxzy0123456789", 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        var configuration = new Configuration
        {
            Version = 1,
            DataDirectory = Path.Join(workingDirectory, "UserUploadDirectory"),
            DataKey = dataKey,
            Database = dbConfig
        };

        var fileName = Path.Join(workingDirectory, "GlobalConfiguration.json");
        await using var createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, configuration,
            new JsonSerializerOptions {WriteIndented = true});
        await createStream.DisposeAsync();
        Console.WriteLine("Configuration created. Exiting... " +
                          "\nPlease check the configuration, confirm and restart the application.");
        Environment.Exit(1);
    }

    public static Configuration ReadConfiguration()
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fileName = Path.Join(workingDirectory, "GlobalConfiguration.json");
        var jsonString = File.ReadAllText(fileName);
        var configuration = JsonSerializer.Deserialize<Configuration>(jsonString);

        if (configuration == null)
        {
            Console.WriteLine("Configuration empty? Delete it for recreation.");
            Environment.Exit(-1);
        }

        var newConfigVersion = new Configuration().Version;

        if (newConfigVersion > configuration.Version)
        {
            MigrateConfiguration(configuration, newConfigVersion);
        }

        return configuration;
    }

    private static async void MigrateConfiguration(Configuration configuration, byte newConfigVersion)
    {
        var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        File.Delete(Path.Join(workingDirectory, "GlobalConfiguration.json"));

        var configPostMig = new Configuration
        {
            Version = newConfigVersion,
            DataDirectory = configuration.DataDirectory,
            DataKey = configuration.DataKey,
            Database = configuration.Database
        };

        var fileName = Path.Join(workingDirectory, "GlobalConfiguration.json");
        await using var createStream = File.Create(fileName);
        await JsonSerializer.SerializeAsync(createStream, configPostMig,
            new JsonSerializerOptions {WriteIndented = true});
        await createStream.DisposeAsync();
        Console.WriteLine("Configuration migrated. Exiting... " +
                          "\nPlease check the configuration, confirm and restart the application.");
        Environment.Exit(2);
    }
}
