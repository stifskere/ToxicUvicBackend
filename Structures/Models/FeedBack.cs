
namespace ToxicUvicBackend.Structures.Models;

public class FeedBack
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public VoteType VoteType { get; set; }
    public string IpAddress { get; set; } = default!;
    public DateTime SentAt { get; set; } = default!;
    
    public Post? Post { get; set; }
}

public enum VoteType
{
    UpVote = 1,
    DownVote = 2
}