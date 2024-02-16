using MemwLib.Data.EnvironmentVariables.Attributes;

namespace ToxicUvicBackend.Structures;

public class EnvConfig
{
    [EnvironmentVariable("MYSQL_SERVER")] 
    public string DatabaseHostName { get; set; } = default!;

    [EnvironmentVariable("MYSQL_DATABASE")]
    public string DatabaseName { get; set; } = default!;

    [EnvironmentVariable("MYSQL_USERNAME")]
    public string DatabaseUsername { get; set; } = default!;

    [EnvironmentVariable("MYSQL_PASSWORD")]
    public string DatabasePassword { get; set; } = default!;
}