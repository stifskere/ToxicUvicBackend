using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace ToxicUvicBackend.Structures.Models;

[Table("feedbacks")]
public class Feedback
{
    [Column("id"), Key]
    public long Id { get; init; } // id INTEGER PRIMARY KEY AUTO_INCREMENT
    
    [Column("post_id")]
    public long PostId { get; init; } // post_id INTEGER REFERENCES posts(id)
    
    [Column("vote_type"), Required]
    public VoteType VoteType { get; set; } // vote_type INTEGER NOT NULL
    
    [Column("sent_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime SentAt { get; init; } // sent_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
    
    [Column("session_token_id")] 
    public long SessionTokenId { get; init; } // session_token_id INTEGER REFERENCES session_tokens(id)
    
    [ForeignKey(nameof(SessionTokenId))]
    public SessionToken SessionToken { get; init; } = default!;
}

[JsonConverter(typeof(StringEnumConverter))]
public enum VoteType
{
    [EnumMember(Value = "up")]
    UpVote = 1,
    
    [EnumMember(Value = "down")]
    DownVote = 2
}