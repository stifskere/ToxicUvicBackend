using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;
using ToxicUvicBackend.Structures.JsonObjects.DataStructures;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/feed"), UsedImplicitly]
[UsesMiddleware(typeof(Tokens), nameof(Tokens.TokenCheckerMiddleware))]
public class Feed
{
    private static readonly DatabaseContext Connection =
        Program.Services.GetRequiredService<DatabaseContext>();
    
    [GroupMember(RequestMethodType.Get, "/length/?", true), UsedImplicitly]
    public static ResponseEntity GetLength(RequestEntity request)
    {
        string? query = request.Path.Parameters["query"];
        string[]? categories = request.Path.Parameters["categories"]?
            .Split(',')
            .Select(c => c.Trim())
            .ToArray();
        
        IEnumerable<Post> result = Connection.Posts
            .AsEnumerable()
            .Where(p => categories == null || p.Categories.Split(',').Any(c => categories.Contains(c.Trim())))
            .Where(p => query == null || p.Message.Contains(query));
        
        return new ResponseEntity(
            ResponseCodes.Ok, 
            new JsonBody<BaseResponse<CountResponse>>(
                new BaseResponse<CountResponse>
                {
                    Success = true,
                    Content = new CountResponse(result.Count())
                }
            )
        );
    }

    [GroupMember(RequestMethodType.Get, "/?", true), UsedImplicitly]
    public static ResponseEntity GetPosts(RequestEntity request)
    {
        if (!int.TryParse(request.Path.Parameters["from"], null, out int from)
            || !int.TryParse(request.Path.Parameters["to"], null, out int to))
        {
            return new ResponseEntity(
                ResponseCodes.BadRequest,
                BaseResponse<string>
                    .MakeErrorResponse("'from' or 'to' properties are missing, request count first.")
            );
        }

        string? query = request.Path.Parameters["query"];
        SessionToken token = (SessionToken)request.SessionParameters["Token"];
        string[]? categories = request.Path.Parameters["categories"]?
            .Split(',')
            .Select(c => c.Trim())
            .ToArray();
        
        List<PublicPost> posts = Connection.Posts
            .Include(p => p.SessionToken)
            .Include(p => p.Attachments) 
            .Include(p => p.FeedBacks)
            .ThenInclude(f => f.SessionToken)
            .OrderByDescending(p => p.CreatedAt)
            .AsEnumerable()
            .Where(p => categories == null || p.Categories.Split(',').Any(c => categories.Contains(c.Trim())))
            .Where(p => query == null || p.Message.Contains(query))
            .Skip(from)
            .Take(to - from)
            .Select(p => new PublicPost(p, token.Token))
            .ToList();
        
        return new ResponseEntity(
            ResponseCodes.Ok, 
            BaseResponse<List<PublicPost>>
                .MakeSuccessResponse(posts)
        );
    }
}