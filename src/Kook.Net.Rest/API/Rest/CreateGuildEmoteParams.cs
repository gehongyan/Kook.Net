using Kook.Net.Rest;

namespace Kook.API.Rest;

internal class CreateGuildEmoteParams
{
    public string? Name { get; set; }
    public required ulong GuildId { get; set; }
    public required Stream File { get; set; }

    public IReadOnlyDictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dic = new()
        {
            ["guild_id"] = GuildId
        };
        if (Name is not null)
            dic["name"] = $"{Name}";

        string contentType = File is FileStream fileStream
            ? $"image/{Path.GetExtension(fileStream.Name)}"
            : "image/png";
        dic["emoji"] = new MultipartFile(File, Name ?? "image", contentType.Replace(".", ""));

        return dic;
    }
}
