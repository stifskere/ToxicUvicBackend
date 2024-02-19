
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.JsonObjects.DataStructures;

public class RequestPost
{
    [JsonProperty("username")]
    public string? Username { get; init; }
    
    [JsonProperty("message"), UsedImplicitly] 
    public string Message { get; init; } = default!;

    [JsonProperty("categories"), UsedImplicitly] 
    public string[] Categories { get; init; } = [];

    [JsonProperty("attachments"), UsedImplicitly]
    public string[] Attachments { get; init; } = [];
}