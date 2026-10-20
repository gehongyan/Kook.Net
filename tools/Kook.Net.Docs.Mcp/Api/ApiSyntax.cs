using YamlDotNet.Serialization;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     API 条目的语法声明。
/// </summary>
public sealed class ApiSyntax
{
    /// <summary>C# 声明签名。</summary>
    [YamlMember(Alias = "content")]
    public string? Content { get; set; }

    /// <summary>参数列表。</summary>
    [YamlMember(Alias = "parameters")]
    public List<ApiParameter>? Parameters { get; set; }

    /// <summary>返回值说明。</summary>
    [YamlMember(Alias = "return")]
    public ApiParameter? Return { get; set; }
}
