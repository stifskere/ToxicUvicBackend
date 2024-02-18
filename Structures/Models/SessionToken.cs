using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToxicUvicBackend.Structures.Models;

[Table("session_tokens")]
public class SessionToken
{
    [Column("id"), Key] 
    public long Id { get; init; } // id INTEGER PRIMARY KEY AUTO_INCREMENT

    [Column("token"), MaxLength(30), Required] 
    public string Token { get; init; } = default!; // token VARCHAR(30) NOT NULL
    
    [Column("created_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime CreatedAt { get; init; } // created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
    
    [Column("expires_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
    public DateTime ExpiresAt { get; init; } // expires_at TIMESTAMP NOT NULL DEFAULT (CURRENT_TIMESTAMP + INTERVAL 1 WEEK)

    public static bool operator ==(SessionToken left, SessionToken right) 
        => left.Token == right.Token;

    public static bool operator !=(SessionToken left, SessionToken right) 
        => left.Token != right.Token;
    
    protected bool Equals(SessionToken other) 
        => Token == other.Token;

    public override bool Equals(object? obj) 
        => !ReferenceEquals(null, obj) &&
           (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((SessionToken)obj));

    public override int GetHashCode() 
        => HashCode.Combine(Id, Token, CreatedAt, ExpiresAt);
}