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

    /// <summary>DocFX 已提取的正文搜索文本，保留原始文本中的泛型等符号。</summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>旧版索引的正文 / 关键字文本。</summary>
    [JsonPropertyName("keywords")]
    public string? Keywords { get; set; }

    internal string? Content => !string.IsNullOrWhiteSpace(Summary)
        ? Summary
        : !string.IsNullOrWhiteSpace(Keywords)
            ? Keywords
            : null;
}
