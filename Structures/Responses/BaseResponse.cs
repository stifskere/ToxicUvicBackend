using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.Responses;

[JsonObject(MemberSerialization.Fields)]
public class BaseResponse<TContent>
{
    [JsonProperty("success")] public bool Success { get; set; }
    
    [JsonProperty("content")] public TContent? Content { get; set; }
}