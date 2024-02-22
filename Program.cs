using MemwLib.Data.EnvironmentVariables;
using MemwLib.Http;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.SSL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToxicUvicBackend.Services;
using ToxicUvicBackend.Structures;

#if DEBUG
using MemwLib.Http.Types;
#endif

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
            SslBehavior = SslBehavior.DoNotUseCertificateIfNotFound,
#if DEBUG
            ServerState = ServerStates.Development,
            Port = 10001
#else
            Port = 80
#endif
        });

#if DEBUG
        EnvConfig env = new EnvContext()
            .AddVariablesFrom(File.Open("./.env", FileMode.Open), true)
#else
        EnvConfig env = new EnvContext(true)
#endif
            .ToType<EnvConfig>(true);
        
        HostApplicationBuilder builder = new();

        builder.Services
            .AddSingleton(server)
            .AddSingleton(env)
            .AddDbContext<DatabaseContext>(dbContextBuilder =>
            {
                string connectionString
                    = $"Server={env.DatabaseHostName};Database={env.DatabaseName};Uid={env.DatabaseUsername};Pwd={env.DatabasePassword}";
                
                dbContextBuilder.UseMySql(connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    options =>
                    {
                        options.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null
                        );

                        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                );
            })
            .AddHostedService<ServerService>();

        return builder.Build();
    }
}