using MemwLib.Http.Types.Entities;
using ToxicUvicBackend.Structures.JsonObjects;

namespace ToxicUvicBackend.Routes;

public static class Middleware
{
    public static IResponsible TokenCheckerMiddleware(RequestEntity request)
    {
        if (!request.Headers.Contains("Host"))
            return new ResponseEntity(
                ResponseCodes.Unauthorized, 
                BaseResponse<string>.MakeErrorResponse("Missing host header.")
            );

        return new NextMiddleWare();
    }
}