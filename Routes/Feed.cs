using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using Microsoft.Extensions.DependencyInjection;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/feed"), UsedImplicitly]
[UsesMiddleware(typeof(Feed), nameof(HostCheckerMiddleware))]
public class Feed
{
    private static readonly IPostRepository PostRepository 
        = Program.Services.GetRequiredService<IPostRepository>();
    
    [GroupMember(RequestMethodType.Get, "/length"), UsedImplicitly]
    public static ResponseEntity GetLength(RequestEntity request)
    {
        BaseResponse<CountResponse> response = new()
        {
            Success = true,
            Content = new CountResponse(
                PostRepository.GetInsightPostCount(request.Path.Parameters["category"])
            )
        }; 
        
        return new ResponseEntity(
            ResponseCodes.Ok, 
            new JsonBody<BaseResponse<CountResponse>>(response)
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
                BaseResponse<string>.MakeErrorResponse("'from' or 'to' properties are missing, request count first.")
            );
        }
        
        List<PostResponse> posts = PostRepository.GetInsightPosts(from, to)
            .Select(p => new PostResponse(request.Headers["Host"]!)
            {
                Attachments = p.Attachments,
                Category = p.Category,
                Feedback = p.Feedback,
                Id = p.Id,
                Message = p.Message,
                CreatedAt = p.CreatedAt
            })
            .ToList();

        if (request.Path.Parameters.Contains("category"))
        {
            posts = posts
                .Where(p => p.Category == request.Path.Parameters["category"])
                .ToList();
        }

        if (to - from >= posts.Count)
        {
            return new ResponseEntity(
                ResponseCodes.BadRequest,
                BaseResponse<string>.MakeErrorResponse("'from' to 'to' range is out of range, request count first.")
            );
        }
        
        return new ResponseEntity(
            ResponseCodes.Ok, 
            new JsonBody<BaseResponse<List<PostResponse>>>(new BaseResponse<List<PostResponse>>
            {
                Success = true,
                Content = posts
            })
        );
    }
    
    public static IResponsible HostCheckerMiddleware(RequestEntity request)
    {
        if (!request.Headers.Contains("Host"))
            return new ResponseEntity(
                ResponseCodes.Unauthorized, 
                BaseResponse<string>.MakeErrorResponse("Missing host header.")
            );

        return new NextMiddleWare();
    }
}