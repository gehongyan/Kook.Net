using YamlDotNet.Serialization;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     docfx <c>source.remote</c> 块，记录源码对应的远程仓库坐标，用于拼接在线查看链接。
/// </summary>
public sealed class ApiSourceRemote
{
    /// <summary>仓库相对路径，如 <c>src/Kook.Net.Core/.../AlphaColor.cs</c>。</summary>
    [YamlMember(Alias = "path")]
    public string? Path { get; set; }

    /// <summary>分支名。</summary>
    [YamlMember(Alias = "branch")]
    public string? Branch { get; set; }

    /// <summary>仓库地址，如 <c>https://github.com/gehongyan/Kook.Net.git</c>。</summary>
    [YamlMember(Alias = "repo")]
    public string? Repo { get; set; }
}
