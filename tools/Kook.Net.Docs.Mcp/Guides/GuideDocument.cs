namespace Kook.Net.Docs.Mcp.Guides;

/// <summary>
///     一篇概念 / 指南类 Markdown 文档的条目。
/// </summary>
/// <param name="RelativePath">相对于 docfx 工程根目录的路径（统一用 <c>/</c> 分隔），用作读取时的标识。</param>
/// <param name="Category">所属分组：<c>guides</c> / <c>quick_reference</c> / <c>demos</c> / <c>changelog</c>。</param>
/// <param name="Title">从首个 Markdown 一级标题提取的标题，提取不到时用文件名。</param>
public sealed record GuideDocument(string RelativePath, string Category, string Title);
