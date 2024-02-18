using System.Security.Cryptography;
using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;
using ToxicUvicBackend.Structures.JsonObjects.DataStructures;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/tokens")]
public class Tokens
{
    private static readonly DatabaseContext Connection =
        Program.Services.GetRequiredService<DatabaseContext>();

    [GroupMember(RequestMethodType.Get, "/?", true), UsedImplicitly]
    public static ResponseEntity GetNewToken(RequestEntity request)
    {
        string randomBytes = Convert.ToBase64String(RandomNumberGenerator.GetBytes(30));

        if (randomBytes.Length > 30)
            randomBytes = randomBytes[..30];

        EntityEntry<SessionToken> token = Connection.SessionTokens
            .Add(new SessionToken { Token = randomBytes });

        Connection.SaveChanges();
        
        return new ResponseEntity(
            ResponseCodes.Ok,
            BaseResponse<PublicToken>.MakeSuccessResponse(new PublicToken(token.Entity))
        );
    }

    public static IResponsible TokenCheckerMiddleware(RequestEntity request)
    {
        if (!request.Headers.Contains("Authorization"))
            return new ResponseEntity(
                ResponseCodes.Unauthorized, 
                BaseResponse<string>.MakeErrorResponse("Missing Authorization header.")
            );

        string header = request.Headers["Authorization"]!.Trim();
        
        SessionToken? token = Connection.SessionTokens
            .FirstOrDefault(k => k.Token == header && k.ExpiresAt > DateTime.Now);
        
        if (token is null)
            return new ResponseEntity(
                ResponseCodes.Unauthorized,
                BaseResponse<string>.MakeErrorResponse(
                    "Provided authorization does not exist or already expired, please request a new token."
                )
            );
        
        request.SessionParameters.Set("Token", token);
        
        return new NextMiddleWare();
    }
}