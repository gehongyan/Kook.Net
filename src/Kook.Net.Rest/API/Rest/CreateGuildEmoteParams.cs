using Kook.Net.Rest;

namespace Kook.API.Rest;

internal class CreateGuildEmoteParams
{
    public string Name { get; set; }
    public ulong GuildId { get; set; }
    public Stream File { get; set; }
    
    public IReadOnlyDictionary<string, object> ToDictionary()
    {
        var d = new Dictionary<string, object>
        {
            ["guild_id"] = GuildId
        };

        if (Name is not null)
            d["name"] = $"{Name}";
        
        string contentType = "image/png";

        if (File is FileStream fileStream)
            contentType = $"image/{Path.GetExtension(fileStream.Name)}";

        d["emoji"] = new MultipartFile(File, Name ?? "image", contentType.Replace(".", ""));

        return d;
    }
}