using System.Text.Json.Serialization;

namespace Kook.Net.Docs.Mcp.Search;

/// <summary>
///     检索索引 JSON 的源生成上下文（便于后续 AOT / 裁剪发布）。
/// </summary>
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Dictionary<string, SearchIndexEntry>))]
internal partial class DocsJsonContext : JsonSerializerContext;
