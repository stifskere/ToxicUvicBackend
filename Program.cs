
using MemwLib.Http;
using MemwLib.Http.Types.Configuration;
using MemwLib.Http.Types.SSL;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToxicUvicBackend.Services;

namespace ToxicUvicBackend;

internal static class Program
{
    private static IHost Provider { get; } = CreateProvider();

    public static IServiceProvider Services => Provider.Services;
    
    private static void Main()
    {
        Provider.Run();
    }

    private static IHost CreateProvider()
    {
        HttpServer server = new(new HttpServerConfig
        {
            Port = 10001,
            SslBehavior = SslBehavior.DoNotUseCertificateIfNotFound
        });
        
        HostApplicationBuilder builder = new();

        builder.Services
            .AddSingleton(server)
            .AddHostedService<DatabaseService>()
            .AddHostedService<RoutesService>();

        return builder.Build();
    }
}