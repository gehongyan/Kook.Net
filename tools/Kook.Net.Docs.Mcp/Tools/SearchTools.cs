using System.ComponentModel;
using System.Text;
using Kook.Net.Docs.Mcp.Search;
using ModelContextProtocol.Server;

namespace Kook.Net.Docs.Mcp.Tools;

/// <summary>
///     全文检索相关的 MCP 工具。
/// </summary>
[McpServerToolType]
public sealed class SearchTools
{
    private readonly SearchIndexService _search;

    /// <summary>
    ///     构造检索工具，注入检索服务。
    /// </summary>
    public SearchTools(SearchIndexService search) => _search = search;

    /// <summary>
    ///     在整个 Kook.Net 文档站（API 与指南）中做全文检索。
    /// </summary>
    [McpServerTool(Name = "search_docs")]
    [Description("在 Kook.Net 文档站中全文检索，返回最相关的页面标题、链接与摘要片段。适合按关键字找 API 或指南。多个关键字按空格分词、以「或」匹配：命中词越多、命中标题的排得越前；加入过于宽泛的词可能引入更多弱相关结果。")]
    public string SearchDocs(
        [Description("查询关键字，可以是类型名、成员名或概念词，如 \"IGuild 禁言\" 或 \"card message\"。多词之间空格分隔，按「或」匹配。")]
        string query,
        [Description("最大返回条数，默认 20。")]
        int limit = 20)
    {
        IReadOnlyList<SearchResult> results = _search.Search(query, limit);
        if (results.Count == 0)
            return $"没有找到与 \"{query}\" 相关的结果。";

        StringBuilder sb = new();
        sb.AppendLine($"找到 {results.Count} 条与 \"{query}\" 相关的结果：");
        sb.AppendLine();
        foreach (SearchResult r in results)
        {
            sb.AppendLine($"- {r.Title}  (score={r.Score:0})");
            sb.AppendLine($"  链接：{r.Href}");
            if (!string.IsNullOrWhiteSpace(r.Snippet))
                sb.AppendLine($"  摘要：{r.Snippet}");
        }

        return sb.ToString().TrimEnd();
    }
}
