using YamlDotNet.Serialization;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     单个 docfx ManagedReference YAML 文件（<c>api/*.yml</c>）的顶层结构。
/// </summary>
public sealed class ApiFile
{
    /// <summary>该文件声明的 API 条目（首个通常是类型或命名空间本身，其余为其成员）。</summary>
    [YamlMember(Alias = "items")]
    public List<ApiItem> Items { get; set; } = [];
}
