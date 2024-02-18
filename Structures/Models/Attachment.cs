using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace ToxicUvicBackend.Structures.Models;

[Table("attachments")]
public class Attachment
{
    [Column("id"), Key]
    public long Id { get; init; } // id INTEGER PRIMARY_KEY AUTO_INCREMENT
    
    [Column("post_id")]
    public long PostId { get; init; } 
    
    [Column("index"), Required] // index INTEGER NOT NULL
    public long Index { get; init; }
}

public static class AttachmentsManager
{
    private static void RemovePrefix(ref string data)
    {
        if (!data.StartsWith("data:image/")) 
            return;
        
        int idxOfEnd = data.IndexOf(';');
        data = data[((idxOfEnd == -1 ? 0 : idxOfEnd) + 1)..];
    }
    
    public static bool ValidateImage(string data, [NotNullWhen(true)]out string? fmt)
    {
        RemovePrefix(ref data);
        
        IImageFormat format;

        try
        {
            Console.WriteLine(data);
            format = Image.DetectFormat(new MemoryStream(Convert.FromBase64String(data)));
        }
        catch
        {
            fmt = null;
            return false;
        }

        fmt = format.Name;

        Console.WriteLine(fmt);
        
        return format.Name is "JPG" or "JPEG" or "PNG";
    }
    
    public static int SaveImage(string data, long postId, string? fmt = null)
    {
        RemovePrefix(ref data);

        if (fmt is null && !ValidateImage(data, out fmt))
            return -1;
        
        using MemoryStream image = new MemoryStream(Convert.FromBase64String(data));

        string path = Path.Join(".", "Attachments");
        
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        int index = Directory.GetFiles(path, $"{postId}-*").Length;

        if (index > 5)
            return -1;
        
        using FileStream fileStream 
            = new(Path.Join(path, $"{postId}-{index}.{fmt.ToLower()}"), FileMode.CreateNew);
        image.CopyTo(fileStream);

        return index;
    }

    public static string? GetImageInBase64(long postId, long index)
    {
        string? path = Directory.GetFiles(Path.Join(".", "Attachments"))
            .FirstOrDefault(p => p.Contains($"{postId}-{index}"));

        if (path is null)
            return null;
        
        using FileStream file = File.Open(path, FileMode.Open);
        byte[] buffer = new byte[file.Length];
        _ = file.Read(buffer, 0, buffer.Length);

        return $"data:image/{path.Split('.')[^1]};base64,{Convert.ToBase64String(buffer)}";
    }
}