using Newtonsoft.Json;

namespace ToxicUvicBackend.Structures.JsonObjects;

public class BaseResponse<TContent>
{
    [JsonProperty("success")] public required bool Success { get; set; }
    
    [JsonProperty("content")] public required TContent? Content { get; set; }

    public static JsonBody<BaseResponse<string>> MakeErrorResponse(string error)
        => new(new BaseResponse<string>
        {
            Success = false,
            Content = error
        });

    public static JsonBody<BaseResponse<T>> MakeSuccessResponse<T>(T content)
        => new(new BaseResponse<T>
        {
            Success = true,
            Content = content
        });
}