using Microsoft.Extensions.Hosting;

namespace ToxicUvicBackend.Services;

public class DatabaseService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}