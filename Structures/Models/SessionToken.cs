using System.ComponentModel.DataAnnotations.Schema;

namespace ToxicUvicBackend.Structures.Models;

public class SessionToken
{
    [Column("session_token_id")] 
    public int Id { get; set; } // id INTEGER PRIMARY KEY AUTO_INCREMENT

    [Column("session_token")] 
    public string Token { get; set; } = default!; // token VARCHAR(30) NOT NULL
    
    [Column("session_token_created_at")] 
    public DateTime CreatedAt { get; set; } // created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
    
    [Column("session_token_expires_at")] 
    public DateTime ExpiresAt { get; set; } // expires_at TIMESTAMP NOT NULL DEFAULT (CURRENT_TIMESTAMP + INTERVAL 1 WEEK)
}