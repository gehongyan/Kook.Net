using YamlDotNet.Serialization;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     docfx ManagedReference YAML（<c>api/*.yml</c>）中 <c>items</c> 下的一个 API 条目，
///     对应一个命名空间 / 类型 / 成员。
/// </summary>
/// <remarks>
///     仅映射检索与展示需要的字段，未知字段由 YamlDotNet 忽略。
/// </remarks>
public sealed class ApiItem
{
    /// <summary>唯一标识符（docfx uid），如 <c>Kook.IGuild.GetUserAsync*</c>。</summary>
    [YamlMember(Alias = "uid")]
    public string Uid { get; set; } = string.Empty;

    /// <summary>短名称，如 <c>GetUserAsync</c>。</summary>
    [YamlMember(Alias = "name")]
    public string? Name { get; set; }

    /// <summary>带声明类型的名称，如 <c>IGuild.GetUserAsync</c>。</summary>
    [YamlMember(Alias = "nameWithType")]
    public string? NameWithType { get; set; }

    /// <summary>完全限定名。</summary>
    [YamlMember(Alias = "fullName")]
    public string? FullName { get; set; }

    /// <summary>条目类型：<c>Namespace</c> / <c>Class</c> / <c>Interface</c> / <c>Method</c> / <c>Property</c> 等。</summary>
    [YamlMember(Alias = "type")]
    public string? Type { get; set; }

    /// <summary>所属命名空间。</summary>
    [YamlMember(Alias = "namespace")]
    public string? Namespace { get; set; }

    /// <summary>所属父条目的 uid（成员的父类型、类型的父命名空间）。</summary>
    [YamlMember(Alias = "parent")]
    public string? Parent { get; set; }

    /// <summary>摘要说明（已渲染为 HTML / 纯文本混合）。</summary>
    [YamlMember(Alias = "summary")]
    public string? Summary { get; set; }

    /// <summary>补充说明。</summary>
    [YamlMember(Alias = "remarks")]
    public string? Remarks { get; set; }

    /// <summary>语法声明（含 C# 代码签名）。</summary>
    [YamlMember(Alias = "syntax")]
    public ApiSyntax? Syntax { get; set; }

    /// <summary>源码位置（定义所在文件与起始行），用于按 uid 读取源码片段。</summary>
    [YamlMember(Alias = "source")]
    public ApiSource? Source { get; set; }

    /// <summary>子条目 uid 列表（命名空间下的类型、类型下的成员）。</summary>
    [YamlMember(Alias = "children")]
    public List<string>? Children { get; set; }

    /// <summary>示例代码块。</summary>
    [YamlMember(Alias = "example")]
    public List<string>? Example { get; set; }
}
