using MemwLib.Http;
using MemwLib.Http.Types.Entities;
using Microsoft.Extensions.Hosting;
using ToxicUvicBackend.Routes;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;

namespace ToxicUvicBackend.Services;

public class ServerService(HttpServer server) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        server.OnLog += message => Console.WriteLine(message);

        server.AddResponseListener(ResponseCodes.NotFound, _ => new ResponseEntity(
            ResponseCodes.NotFound, 
            new JsonBody<BaseResponse<string>>(new BaseResponse<string> { Success = false, Content = "Not found." })
        ));
        
        server.AddGroup<Feed>();
        
        return Task.CompletedTask;
    }
    
    public override Task StopAsync(CancellationToken ct)
    {
        base.StopAsync(ct);
        server.Dispose();
        return Task.CompletedTask;
    }
}