
using Insight.Database;
using Insight.Database.Providers;
using Insight.Database.Providers.MySql;
using MemwLib.Data.EnvironmentVariables;
using MemwLib.Http;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.SSL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using ToxicUvicBackend.Services;
using ToxicUvicBackend.Structures;

namespace ToxicUvicBackend;

internal static class Program
{
    private static IHost Provider { get; } = CreateProvider();

    public static IServiceProvider Services => Provider.Services;
    
    private static Task Main()
    {
        return Provider.RunAsync();
    }

    private static IHost CreateProvider()
    {
        HttpServer server = new(new HttpServerConfig
        {
            Port = 10001,
            SslBehavior = SslBehavior.DoNotUseCertificateIfNotFound,
#if DEBUG
            ServerState = ServerStates.Development
#endif
        });

        EnvConfig env = new EnvContext()
            .AddVariablesFrom(File.Open("./.env", FileMode.Open), true)
            .ToType<EnvConfig>(true);
        
        InsightDbProvider.RegisterProvider(new MySqlInsightDbProvider());
        
        MySqlConnection connection = new(
            $"Server={env.DatabaseHostName};Database={env.DatabaseName};Uid={env.DatabaseUsername};Pwd={env.DatabasePassword}"
        );
        
        connection.Open();
        
        HostApplicationBuilder builder = new();

        builder.Services
            .AddSingleton(server)
            .AddSingleton(env)
            .AddSingleton(connection.As<IPostRepository>())
            .AddHostedService<ServerService>();

        return builder.Build();
    }
}