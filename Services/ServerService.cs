using System.Text.RegularExpressions;
using MemwLib.Http;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Entities;
using MemwLib.Http.Types.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ToxicUvicBackend.Routes;
using ToxicUvicBackend.Structures.JsonObjects;

#if !DEBUG
using ToxicUvicBackend.Structures;
#endif

namespace ToxicUvicBackend.Services;

public class ServerService(HttpServer server, ILoggerFactory loggerFactory) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        ILogger<ServerService> logger = loggerFactory.CreateLogger<ServerService>();
        int logId = 0;
        server.OnLog += message =>
        {
            logger.Log(
                message.Type switch
                {
                    LogType.Error => LogLevel.Critical,
                    LogType.Info => LogLevel.Information,
                    LogType.Warning => LogLevel.Warning,
                    LogType.FailedRequest => LogLevel.Warning,
                    LogType.SuccessfulRequest => LogLevel.Information,
                    _ => LogLevel.None
                },
                new EventId(logId++),
                "[{Time:MM/dd/yyyy HH:mm:ss}] {Message}",
                message.Date,
                message.Message
            );
        };

        server.AddResponseListener(ResponseCodes.MethodNotAllowed, (request, _) 
            => new ResponseEntity(ResponseCodes.NotFound, BaseResponse<string>.MakeErrorResponse($"{request.Path.Route} was not found.")));
        
        server.AddResponseListener(ResponseCodes.NotFound, (_, response) =>
        {
            if (response.Body.ContentType is not "application/json")
                return new ResponseEntity(
                    ResponseCodes.NotFound,
                    BaseResponse<string>.MakeErrorResponse("Not found.")
                );

            return response;
        });

#if !DEBUG
        server.AddResponseListener(ResponseCodes.InternalServerError, (_, _) => new ResponseEntity(
            ResponseCodes.InternalServerError,
            BaseResponse<string>.MakeErrorResponse("Internal server error, check logs")
        ));
#endif
        server.AddGroup<Feed>();
        server.AddGroup<Tokens>();
        server.AddGroup<Posts>();
        server.AddGroup<Categories>();

        // cors shit
        server.AddEndpoint(RequestMethodType.Options, new Regex(".+"), request =>
            {
                if (!request.Headers.Contains("Access-Control-Request-Method"))
                    return new ResponseEntity(ResponseCodes.NotAcceptable);

                return new ResponseEntity(ResponseCodes.Ok)
                    .WithHeader("Access-Control-Allow-Methods", request.Headers["Access-Control-Request-Method"]!);
            }
        );
        
        server.AddGlobalMiddleware(_ => new NextMiddleWare()
            .WithHeader("Access-Control-Allow-Origin", 
#if DEBUG
                "http://localhost:8080"
#else
                "https://toxicuvic.es"
#endif  
            )
            .WithHeader("Access-Control-Allow-Headers", "Authorization")
        );
        
        return Task.CompletedTask;
    }
    
    public override Task StopAsync(CancellationToken ct)
    {
        base.StopAsync(ct);
        server.Dispose();
        return Task.CompletedTask;
    }
}