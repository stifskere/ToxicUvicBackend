
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.JsonObjects.DataStructures;

public class RequestPost
{
    [JsonProperty("message"), UsedImplicitly] 
    public string Message { get; init; } = default!;

    [JsonProperty("categories"), UsedImplicitly] 
    public string[] Categories { get; init; } = default!;

    [JsonProperty("attachments"), UsedImplicitly]
    public string[] Attachments { get; init; } = [];
}