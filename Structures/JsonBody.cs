using MemwLib.Http.Types.Content;
using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures;

public class JsonBody<TStructure>(TStructure content) : IBody
{
    public TStructure Content => content;
    
    public static IBody ParseImpl(string content)
        => new JsonBody<TStructure>(JsonConvert.DeserializeObject<TStructure>(content)!);

    public string ToRaw()
        => JsonConvert.SerializeObject(content);

    public string ContentType => "application/json; charset=utf-8";
}