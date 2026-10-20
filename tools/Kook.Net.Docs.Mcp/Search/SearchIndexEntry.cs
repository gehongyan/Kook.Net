using System.Text.Json.Serialization;

namespace Kook.Net.Docs.Mcp.Search;

/// <summary>
///     docfx <c>index.json</c> 中单个页面的索引记录。
/// </summary>
public sealed class SearchIndexEntry
{
    /// <summary>页面标题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>页面正文提取的关键字文本。</summary>
    [JsonPropertyName("keywords")]
    public string? Keywords { get; set; }
}
