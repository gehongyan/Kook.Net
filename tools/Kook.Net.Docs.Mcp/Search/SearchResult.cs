namespace Kook.Net.Docs.Mcp.Search;

/// <summary>
///     一条全文检索结果。
/// </summary>
/// <param name="Title">页面标题。</param>
/// <param name="Href">相对于站点根的页面链接（如 <c>api/Kook.IGuild.html</c>）。</param>
/// <param name="Score">匹配得分，越大越相关。</param>
/// <param name="Snippet">关键字命中处的摘要片段，可能为空。</param>
public sealed record SearchResult(string Title, string Href, double Score, string? Snippet);
