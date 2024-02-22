using System.Globalization;
using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;
using ToxicUvicBackend.Structures.JsonObjects.DataStructures;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/posts")]
[UsesMiddleware(typeof(Tokens), nameof(Tokens.TokenCheckerMiddleware))]
public class Posts
{
    private static readonly DatabaseContext Connection =
        Program.Services.GetRequiredService<DatabaseContext>();

    private static readonly Dictionary<SessionToken, DateTime> PutPostRateLimits = new();
    
    [GroupMember(RequestMethodType.Put, "/?", true), UsedImplicitly]
    public static ResponseEntity PutNewPost(RequestEntity request)
    {
        RequestPost requestedPost;
        SessionToken token = (SessionToken)request.SessionParameters["Token"];

        if (PutPostRateLimits.TryGetValue(token, out DateTime rateLimitUntil))
        {
            if (rateLimitUntil > DateTime.Now)
            {
                ResponseEntity response = new(
                    // I forgot to put rate limit 429 on MemwLib skull.
                    ResponseCodes.Forbidden,
                    BaseResponse<string>.MakeErrorResponse("You got rate limited, try sending a new request again after: see Retry-After header")
                );
            
                response.Headers.Set("Retry-After", rateLimitUntil.ToString(CultureInfo.InvariantCulture));
            
                return response;
            }

            PutPostRateLimits.Remove(token);
        }
        
        try
        {
            requestedPost = request.Body.ReadAs<JsonBody<RequestPost>>()!.Content;

            if (requestedPost is null)
                throw new NullReferenceException();
        }
        catch
        {
            return new ResponseEntity(
                ResponseCodes.BadRequest, BaseResponse<string>.MakeErrorResponse(
                    "Invalid JSON for a new post, must contain message, categories? properties."
                )
            );
        }

        if (requestedPost.Categories.Any(c => c.Length > 15) || requestedPost.Categories.Length > 5)
            return new ResponseEntity(
                ResponseCodes.BadRequest,
                BaseResponse<string>.MakeErrorResponse(
                    "There must be a maximum of 5 categories with 15 characters maximum each category."
                )
            );

        if (requestedPost.Attachments.Length >= 5)
            return new ResponseEntity(
                ResponseCodes.BadRequest,
                BaseResponse<string>.MakeErrorResponse(
                    "There must be a maximum of 5 attachments per post, the post was not uploaded."    
                )
            );

        List<(string img, string format)> imageTuples = []; 
        
        foreach (string b64Att in requestedPost.Attachments)
        {
            if (!AttachmentsManager.ValidateImage(b64Att, out string? fmt))
                return new ResponseEntity(
                    ResponseCodes.UnsupportedMediaType,
                    BaseResponse<string>.MakeErrorResponse("One of the uploaded attachments is not png|jpg")
                );
            
            imageTuples.Add((b64Att, fmt));
        }
        
        EntityEntry<Post> post = Connection.Posts
            .Add(new Post
                {
                    Message = requestedPost.Message,
                    Categories = string.Join(',', requestedPost.Categories.Select(c => c.Trim())),
                    SessionTokenId = token.Id
                }
            );
        
        Connection.SaveChanges();
        
        foreach ((string img, string format) in imageTuples)
        {
            int index = AttachmentsManager.SaveImage(img, post.Entity.Id, format);
            
            Connection.Attachments
                .Add(new Attachment
                {
                    PostId = post.Entity.Id,
                    Index = index
                });
        }
        
        Connection.SaveChanges();

        PutPostRateLimits.Add(token, DateTime.Now + TimeSpan.FromSeconds(30)); 
        
        return new ResponseEntity(ResponseCodes.Created);
    }

    [GroupMember(RequestMethodType.Get, @"/(?'post_id'[\d]+)/?$", true), UsedImplicitly]
    public static ResponseEntity GetPost(RequestEntity request)
    {
        int postId = int.Parse(request.CapturedGroups["post_id"]);
        
        Post? post = Connection.Posts
            .Include(p => p.SessionToken)
            .Include(p => p.Attachments)
            .Include(p => p.FeedBacks)
            .ThenInclude(f => f.SessionToken)
            .FirstOrDefault(p => p.Id == postId);
        
        if (post is null)
            return new ResponseEntity(
                ResponseCodes.NotFound, 
                BaseResponse<string>.MakeErrorResponse($"No post associated with the id '{postId}'")
            );

        return new ResponseEntity(
            ResponseCodes.Ok,
            BaseResponse<Post>.MakeSuccessResponse(new PublicPost(post, ((SessionToken)request.SessionParameters["Token"]).Token))
        );
    }
    
    [GroupMember(RequestMethodType.Put, @"/(?'post_id'[\d]+)/feedback/?", true), UsedImplicitly]
    public static ResponseEntity PutFeedback(RequestEntity request)
    {
        int postId = int.Parse(request.CapturedGroups["post_id"]);
        string? requestedVoteType = request.Path.Parameters["vote"];
        
        if (requestedVoteType is not ("1" or "2"))
            return new ResponseEntity(
                ResponseCodes.BadRequest,
                BaseResponse<string>.MakeErrorResponse(
                    "A valid vote type was not found, a query parameter called vote must exist with the value 1 (up vote) or 2 (down vote)"
                )
            );
        
        Post? post = Connection.Posts
            .Include(p => p.SessionToken)
            .Include(p => p.FeedBacks)
            .ThenInclude(f => f.SessionToken)
            .FirstOrDefault(p => p.Id == postId);

        if (post is null)
            return new ResponseEntity(
                ResponseCodes.NotFound,
                BaseResponse<string>.MakeErrorResponse($"No post associated with the id '{postId}'")
            );

        SessionToken sessionToken = (SessionToken)request.SessionParameters["Token"];
        
        if (post.SessionToken == sessionToken)
            return new ResponseEntity(
                ResponseCodes.Forbidden,
                BaseResponse<string>.MakeErrorResponse(
                    """
                    You cannot vote your own post.
                    
                    Psst... you can go to another device and vote from there...
                    
                    .. Don't tell anyone that it was me who told you.
                    """
                )
            );

        Feedback? existingFeedback = post.FeedBacks
            .FirstOrDefault(f => f.SessionToken == sessionToken);

        VoteType castedVote = Enum.Parse<VoteType>(requestedVoteType);
        
        if (existingFeedback is not null)
        {
            existingFeedback.VoteType = castedVote;
            Connection.Update(existingFeedback);
        }
        else
        {
            Connection.FeedBacks.Add(new Feedback
            {
                PostId = postId,
                VoteType = castedVote,
                SessionTokenId = sessionToken.Id
            });
        }

        Connection.SaveChanges();
        
        return new ResponseEntity(
            ResponseCodes.NoContent
        );
    }
}