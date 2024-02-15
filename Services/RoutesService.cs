using MemwLib.Http;
using Microsoft.Extensions.Hosting;
using ToxicUvicBackend.Routes;

namespace ToxicUvicBackend.Services;

public class RoutesService(HttpServer server) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        server.OnLog += message => Console.WriteLine(message);

        server.AddGroup<Feed>();
        
        return Task.CompletedTask;
    }
}