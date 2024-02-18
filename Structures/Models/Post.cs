using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToxicUvicBackend.Structures.Models;

[Table("posts")]
public class Post
{
    // actual table columns
    [Column("id"), Key]
    public long Id { get; init; } // id INTEGER PRIMARY KEY AUTO_INCREMENT
    
    [Column("message"), MaxLength(300), Required]
    public string Message { get; init; } = default!; // message VARCHAR(300) NOT NULL
    
    [Column("category"), MaxLength(79)] 
    public string Categories { get; init; } = default!; // category VARCHAR(15)
    
    [Column("created_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime CreatedAt { get; init; } // created_at NOT NULL DEFAULT CURRENT_TIMESTAMP
    
    [Column("session_token_id")]
    public long SessionTokenId { get; init; } // session_token INTEGER REFERENCES session_tokens(id)

    // literal relation sub-objects
    [ForeignKey(nameof(SessionTokenId))]
    public SessionToken SessionToken { get; init; } = default!;
    
    public ICollection<Feedback> FeedBacks { get; init; } = default!;
    
    public ICollection<Attachment> Attachments { get; init; } = default!;
}