
using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.JsonObjects.DataStructures;

public class PublicAttachment
{
    [JsonProperty("data")] 
    public string Data { get; init; } = default!;
    
    [JsonProperty("id")]
    public long Id { get; init; }
}