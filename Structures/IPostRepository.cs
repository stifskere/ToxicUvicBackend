using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Structures;

public interface IPostRepository
{
    long GetInsightPostCount(string? category = null);
    
    IEnumerable<Post> GetInsightPosts(int idFrom, int idTo);

    long GetInsightIsValidToken(string token);
}