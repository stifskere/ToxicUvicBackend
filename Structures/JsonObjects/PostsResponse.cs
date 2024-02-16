using Newtonsoft.Json;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Structures.JsonObjects;

public class PostResponse(string ip) : Post
{
    [JsonProperty("feedback_summary")]
    public PostFeedBackSummary FeedBackSummary
        => new()
        {
            LikeCount = Feedback.Count(f => f.VoteType == VoteType.UpVote),
            DislikeCount = Feedback.Count(f => f.VoteType == VoteType.DownVote),
            CanLike = Feedback.FirstOrDefault(f => f.IpAddress == ip) is null
        };
}

public class PostFeedBackSummary
{
    [JsonProperty("like_count")] public int LikeCount { get; set; }
    [JsonProperty("dislike_count")] public int DislikeCount { get; set; }
    [JsonProperty("can_like")] public bool CanLike { get; set; }
}