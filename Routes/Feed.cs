using Insight.Database;
using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/feed"), UsedImplicitly]
[UsesMiddleware(typeof(Middleware), nameof(Middleware.TokenCheckerMiddleware))]
public class Feed
{
    private static readonly MySqlConnection Connection =
        Program.Services.GetRequiredService<MySqlConnection>();
    
    [GroupMember(RequestMethodType.Get, "/length"), UsedImplicitly]
    public static ResponseEntity GetLength(RequestEntity request)
    {
        long count = Connection.Query<long>(
            "GetInsightPostCount", 
            new { Category = request.Path.Parameters["category"] }
        ).First();
        
        BaseResponse<CountResponse> response = new()
        {
            Success = true,
            Content = new CountResponse(count)
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

        List<Post> posts = Connection.Query(
            "GetInsightPosts",
            new { LFrom = from, LTo = to },
            Query.Returns(Some<Post>.Records)
                .ThenChildren(Some<FeedBack>.Records)
                .ThenChildren(Some<Attachment>.Records)
                .ThenChildren(OneToOne<SessionToken>.Records)
        ).ToList();

        return new ResponseEntity(ResponseCodes.Ok, new JsonBody<List<Post>>(posts));

        // if (request.Path.Parameters.Contains("category"))
        // {
        //     posts = posts
        //         .Where(p => p.Category == request.Path.Parameters["category"])
        //         .ToList();
        // }
        //
        // if (to - from >= posts.Count)
        // {
        //     return new ResponseEntity(
        //         ResponseCodes.BadRequest,
        //         BaseResponse<string>.MakeErrorResponse("'from' to 'to' range is out of range, request count first.")
        //     );
        // }
        //
        // return new ResponseEntity(
        //     ResponseCodes.Ok, 
        //     new JsonBody<BaseResponse<List<PostResponse>>>(new BaseResponse<List<PostResponse>>
        //     {
        //         Success = true,
        //         Content = posts
        //     })
        // );
    }
}