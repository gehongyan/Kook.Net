using YamlDotNet.Serialization;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     方法 / 属性的参数或返回值。
/// </summary>
public sealed class ApiParameter
{
    /// <summary>参数名（返回值无此字段）。</summary>
    [YamlMember(Alias = "id")]
    public string? Id { get; set; }

    /// <summary>参数类型的 uid。</summary>
    [YamlMember(Alias = "type")]
    public string? Type { get; set; }

    /// <summary>参数 / 返回值说明。</summary>
    [YamlMember(Alias = "description")]
    public string? Description { get; set; }
}
