using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/feed"), UsedImplicitly]
public class Feed
{
    [GroupMember(RequestMethodType.Get, "/length"), UsedImplicitly]
    public static ResponseEntity GetLength(RequestEntity entity)
    {
        
        
        return new ResponseEntity(ResponseCodes.Ok);
    }

    [GroupMember(RequestMethodType.Get, "/"), UsedImplicitly]
    public static ResponseEntity GetPosts(RequestEntity entity)
    {

        return new ResponseEntity(ResponseCodes.Ok);
    }
}