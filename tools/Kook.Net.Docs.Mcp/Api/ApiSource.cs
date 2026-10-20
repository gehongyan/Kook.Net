using YamlDotNet.Serialization;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     docfx ManagedReference 条目中的 <c>source</c> 块，记录该类型 / 成员在源码中的位置。
/// </summary>
/// <remarks>
///     docfx 只记录定义的起始行（<see cref="StartLine" />），不记录结束行；
///     读取完整片段的结束边界需由调用方自行推断。
/// </remarks>
public sealed class ApiSource
{
    /// <summary>相对 docfx 工程根的本地源码路径，如 <c>../src/Kook.Net.Core/.../AlphaColor.cs</c>。</summary>
    [YamlMember(Alias = "path")]
    public string? Path { get; set; }

    /// <summary>定义起始行（docfx 为 0 基，对应编辑器行号需 +1）。</summary>
    [YamlMember(Alias = "startLine")]
    public int StartLine { get; set; }

    /// <summary>远程仓库信息（仓库相对路径、分支、仓库地址），用于生成在线链接。</summary>
    [YamlMember(Alias = "remote")]
    public ApiSourceRemote? Remote { get; set; }
}
