namespace ToxicUvicBackend.Structures.Models;

public class Post
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public FeedBack Feedback { get; set; }
    public List<Attachment> Attachments { get; set; }
}