namespace ToxicUvicBackend.Structures.Models;

public class FeedBack
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    
    public Post Post { get; set; }
}