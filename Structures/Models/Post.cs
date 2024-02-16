
using Insight.Database;
using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.Models;

public class Post
{
    [JsonProperty("id")] public long Id { get; set; }
    [JsonProperty("message")] public string Message { get; set; } = default!;
    [JsonProperty("category")] public string Category { get; set; } = default!;
    [JsonProperty("created_at")] public DateTime CreatedAt { get; set; }

    [JsonIgnore] public List<FeedBack> Feedback { get; set; } = [];
    [JsonProperty("attachments")] public List<Attachment> Attachments { get; set; } = [];
}