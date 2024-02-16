using Insight.Database;

namespace ToxicUvicBackend.Structures.Models;

public class FeedBack
{
    [System.ComponentModel.DataAnnotations.Schema.Column("feedback_id")]
    public long Id { get; set; } // id INTEGER PRIMARY KEY AUTO_INCREMENT
    
    [System.ComponentModel.DataAnnotations.Schema.Column("feedback_post_id")]
    public long PostId { get; set; } // post_id INTEGER REFERENCES posts(id)
    
    [System.ComponentModel.DataAnnotations.Schema.Column("feedback_session_token_id")] 
    public long SessionTokenId { get; set; } = default!; // session_token_id INTEGER REFERENCES session_tokens(id)
    
    [System.ComponentModel.DataAnnotations.Schema.Column("feedback_vote_type")]
    public VoteType VoteType { get; set; } // vote_type INTEGER NOT NULL
    
    [System.ComponentModel.DataAnnotations.Schema.Column("feedback_sent_at")]
    public DateTime SentAt { get; set; } // sent_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP

    [ChildRecords]
    public SessionToken SessionToken { get; set; } = default!;
}

public enum VoteType
{
    UpVote = 1,
    DownVote = 2
}