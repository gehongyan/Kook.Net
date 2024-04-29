using Kook.Net.Rest;

namespace Kook.API.Rest;

internal class CreateAssetParams
{
    public required Stream File { get; set; }

    public string? FileName { get; set; }

    public IReadOnlyDictionary<string, object> ToDictionary() =>
        new Dictionary<string, object>
        {
            ["file"] = new MultipartFile(File, FileName ?? GetFilename(File))
        };

    private static string? GetFilename(Stream stream)
    {
        if (stream is FileStream fileStream)
            return Path.GetFileName(fileStream.Name);
        return null;
    }
}
