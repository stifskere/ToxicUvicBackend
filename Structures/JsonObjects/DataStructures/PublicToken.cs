using Newtonsoft.Json;
using ToxicUvicBackend.Structures.Models;

namespace ToxicUvicBackend.Structures.JsonObjects.DataStructures;

public class PublicToken(SessionToken token)
{
    [JsonProperty("token")]
    public string Token { get; init; } = token.Token;

    [JsonProperty("expires_at")]
    public DateTime ExpiresAt { get; init; } = token.ExpiresAt;
}