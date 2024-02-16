using Insight.Database;

namespace ToxicUvicBackend.Structures.Models;

public class Attachment
{
    [System.ComponentModel.DataAnnotations.Schema.Column("attachment_id")]
    public long Id { get; set; } // id INTEGER PRIMARY_KEY AUTO_INCREMENT
    
    [System.ComponentModel.DataAnnotations.Schema.Column("attachment_post_id")]
    public long PostId { get; set; } // post_id INTEGER REFERENCES posts(id)
    
    [System.ComponentModel.DataAnnotations.Schema.Column("attachment_path")] // path VARCHAR(256) NOT NULL
    public string Path { get; set; } = default!;
}