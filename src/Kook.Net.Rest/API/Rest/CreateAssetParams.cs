using Kook.Net.Rest;

namespace Kook.API.Rest;

internal class CreateAssetParams
{
    public Stream File { get; set; }
    public string FileName { get; set; }
    
    public IReadOnlyDictionary<string, object> ToDictionary()
    {
        var d = new Dictionary<string, object>
        {
            ["file"] = new MultipartFile(File, FileName ?? GetFilename(File))
        };
        return d;
    }

    private static string GetFilename(Stream stream)
    {
        if (stream is FileStream fileStream)
            return Path.GetFileName(fileStream.Name);
        return null;
    }
}