
namespace ToxicUvicBackend.Structures.Models;

public class Attachment
{
    public long Id { get; set; }
    public long PostId { get; set; }
    public string Path { get; set; } = default!;
    
    public Post? Post { get; set; }
}