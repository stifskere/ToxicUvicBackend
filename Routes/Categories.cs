using JetBrains.Annotations;
using MemwLib.Http.Types;
using MemwLib.Http.Types.Attributes;
using MemwLib.Http.Types.Entities;
using Microsoft.Extensions.DependencyInjection;
using ToxicUvicBackend.Structures;
using ToxicUvicBackend.Structures.JsonObjects;

namespace ToxicUvicBackend.Routes;

[RouteGroup("/categories")]
[UsesMiddleware(typeof(Tokens), nameof(Tokens.TokenCheckerMiddleware))]
public class Categories
{
    private static readonly DatabaseContext Connection
        = Program.Services.GetRequiredService<DatabaseContext>();
    
    [GroupMember(RequestMethodType.Get, "/usages/?$", true), UsedImplicitly]
    public static ResponseEntity GetCategoryUsage(RequestEntity request)
    {
        Dictionary<string, int> usages = new();

        IEnumerable<string> categories
            = Connection.Posts.AsEnumerable().SelectMany(c => c.Categories.Split(','));
        
        foreach (string category in categories)
        {
            usages.TryAdd(category, 0);
            usages[category]++;
        }

        return new ResponseEntity(
            ResponseCodes.Ok,
            BaseResponse<string>.MakeSuccessResponse(usages)
        );
    }
}