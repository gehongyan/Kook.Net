namespace Kook.Net.Docs.Mcp.Source;

/// <summary>
///     按 uid 读取到的一段源码片段及其定位信息。
/// </summary>
public sealed class SourceSnippet
{
    /// <summary>对应的 API 条目 uid。</summary>
    public required string Uid { get; init; }

    /// <summary>仓库相对路径，如 <c>src/Kook.Net.Core/.../AlphaColor.cs</c>。</summary>
    public required string RepositoryPath { get; init; }

    /// <summary>片段起始行号（1 基，供人阅读）。</summary>
    public required int StartLine { get; init; }

    /// <summary>片段结束行号（1 基，含）。</summary>
    public required int EndLine { get; init; }

    /// <summary>源码文本。</summary>
    public required string Code { get; init; }

    /// <summary>在线查看链接（可用时），形如 <c>https://github.com/owner/repo/blob/branch/path#L9</c>。</summary>
    public string? Url { get; init; }

    /// <summary>结束边界是否因达到最大行数上限而被截断。</summary>
    public bool Truncated { get; init; }

    /// <summary>
    ///     当目标是跨文件 partial 类型时，除本片段所在文件外、该类型其余成员所在的源码文件（仓库相对路径，去重排序）。
    ///     为空表示非 partial，或所有部分都在同一文件。此时本片段可能只覆盖该 partial 类型的一部分。
    /// </summary>
    public IReadOnlyList<string> PartialOtherFiles { get; init; } = [];
}
