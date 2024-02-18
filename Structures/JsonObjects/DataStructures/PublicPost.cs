using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Structures.JsonObjects.DataStructures;

[JsonObject(MemberSerialization.OptIn)]
public class PublicPost
{
    private static readonly EnvConfig EnvConfig
        = Program.Services.GetRequiredService<EnvConfig>();
    
    [JsonProperty("post_id")]
    public int Id { get; init; }
    
    [JsonProperty("categories")]
    public string[] Categories { get; init; }
    
    [JsonProperty("message")]
    public string Message { get; init; }
    
    [JsonProperty("post_date")]
    public DateTime CreatedAt { get; init; }
    
    [JsonProperty("feedback")]
    public VoteStatus Feedback { get; init; }
    
    [JsonProperty("attachments")]
    public PublicAttachment[] Attachments { get; init; }
    
    public PublicPost(Post post, string token)
    {
        Id = (int)post.Id;

        Categories = post.Categories.Split(',');

        Message = post.Message;

        CreatedAt = post.CreatedAt;
        
        Feedback = new VoteStatus
        {
            UpVoteCount = post.FeedBacks.Count(v => v.VoteType == VoteType.UpVote),
            DownVoteCount = post.FeedBacks.Count(v => v.VoteType == VoteType.DownVote),
            CanVote = post.SessionToken.Token != token,
            CastedVote = post.FeedBacks.FirstOrDefault(f => f.SessionToken.Token == token)?.VoteType
        };

        Attachments = post.Attachments
            .Select(a => new PublicAttachment
            {
                Id = a.Id,
                Data = AttachmentsManager.GetImageInBase64(post.Id, a.Index) ?? string.Empty
            })
            .ToArray();
    }
}

public class VoteStatus
{
    [JsonProperty("up_votes")]
    public long UpVoteCount { get; set; }
    
    [JsonProperty("down_votes")]
    public long DownVoteCount { get; set; }
    
    [JsonProperty("can_vote")]
    public bool CanVote { get; set; }
    
    [JsonProperty("casted_vote")]
    public VoteType? CastedVote { get; set; }
}