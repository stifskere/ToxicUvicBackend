
using Insight.Database;
using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.Models;

public class Post
{
    [JsonProperty("id"), Column("post_id")]
    public long Id { get; set; } // id INTEGER PRIMARY KEY AUTO_INCREMENT
    
    [JsonProperty("by"), Column("post_session_token_id")]
    public long SessionTokenId { get; set; } // session_token INTEGER REFERENCES session_tokens(id)
    
    [JsonProperty("message"), Column("post_message")]
    public string Message { get; set; } = default!; // message VARCHAR(300) NOT NULL
    
    [JsonProperty("category"), Column("post_category")] 
    public string Category { get; set; } = default!; // category VARCHAR(15)
    
    [JsonProperty("created_at"), Column("post_created_at")] 
    public DateTime CreatedAt { get; set; } // created_at NOT NULL DEFAULT CURRENT_TIMESTAMP

    
    [JsonIgnore, ChildRecords] 
    public SessionToken SessionToken { get; set; } = default!;
    
    [JsonIgnore, ChildRecords]
    public List<FeedBack> Feedback { get; set; } = [];
    
    [JsonProperty("attachments"), ChildRecords] 
    public List<Attachment> Attachments { get; set; } = [];
}