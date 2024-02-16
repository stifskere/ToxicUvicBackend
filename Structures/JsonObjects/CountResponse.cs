using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.JsonObjects;

public class CountResponse(long postCount)
{
    [JsonProperty("post_count")] public long PostCount => postCount;
}