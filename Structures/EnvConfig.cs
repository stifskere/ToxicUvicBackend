using MemwLib.Data.EnvironmentVariables.Attributes;

namespace ToxicUvicBackend.Structures;

public class EnvConfig
{
    [EnvironmentVariable("MYSQL_SERVER")] 
    public string DatabaseHostName { get; init; } = default!;

    [EnvironmentVariable("MYSQL_DATABASE")]
    public string DatabaseName { get; init; } = default!;

    [EnvironmentVariable("MYSQL_USERNAME")]
    public string DatabaseUsername { get; init; } = default!;

    [EnvironmentVariable("MYSQL_PASSWORD")]
    public string DatabasePassword { get; init; } = default!;

    [EnvironmentVariable("HOST_NAME")]
    public string HostName { get; init; } = default!;
}