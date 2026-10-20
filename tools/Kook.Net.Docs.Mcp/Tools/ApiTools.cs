using System.ComponentModel;
using System.Text;
using Kook.Net.Docs.Mcp.Api;
using ModelContextProtocol.Server;

namespace Kook.Net.Docs.Mcp.Tools;

/// <summary>
///     API 元数据相关的 MCP 工具，数据来源为 docfx 生成的 ManagedReference YAML。
/// </summary>
[McpServerToolType]
public sealed class ApiTools
{
    private readonly ApiModelService _api;

    /// <summary>
    ///     构造 API 工具，注入 API 模型服务。
    /// </summary>
    public ApiTools(ApiModelService api) => _api = api;

    /// <summary>
    ///     按 uid 精确获取一个 API 条目的签名、摘要、参数与返回值。
    /// </summary>
    [McpServerTool(Name = "get_api")]
    [Description("按 docfx uid 精确获取类型或成员的签名、摘要、参数与返回值。uid 可由 find_api 或 search_docs 的链接得到。想看实现源码（而非签名文档）用 get_source。")]
    public string GetApi(
        [Description("API 的 docfx uid，如 \"Kook.IGuild\" 或 \"Kook.IGuild.GetUserAsync(System.UInt64,...)\"。")]
        string uid)
    {
        ApiItem? item = _api.GetByUid(uid);
        if (item is null)
            return $"未找到 uid 为 \"{uid}\" 的 API 条目。可用 find_api 按名称查找。";

        return Format(item, includeMembers: true);
    }

    /// <summary>
    ///     按名称模糊查找 API 条目。
    /// </summary>
    [McpServerTool(Name = "find_api")]
    [Description("按名称模糊查找 API 条目（匹配 name/fullName/uid），可按条目类型过滤。返回候选的 uid 与完全限定名。")]
    public string FindApi(
        [Description("查询词，如 \"GetUserAsync\" 或 \"IGuild\"。")]
        string query,
        [Description("可选条目类型过滤：Namespace / Class / Interface / Struct / Enum / Method / Property / Event / Field。")]
        string? type = null,
        [Description("最大返回条数，默认 20。")]
        int limit = 20)
    {
        IReadOnlyList<ApiItem> items = _api.Find(query, type, limit);
        if (items.Count == 0)
            return $"没有找到与 \"{query}\" 匹配的 API 条目。";

        StringBuilder sb = new();
        sb.AppendLine($"找到 {items.Count} 个匹配 \"{query}\" 的 API 条目：");
        sb.AppendLine();
        foreach (ApiItem i in items)
        {
            sb.AppendLine($"- [{i.Type}] {i.FullName ?? i.Name}");
            sb.AppendLine($"  uid: {i.Uid}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    ///     展开某个类型的直接成员列表。
    /// </summary>
    [McpServerTool(Name = "get_type_members")]
    [Description("列出某个类型（Class/Interface/Struct/Enum/Record/Delegate）的直接成员：方法、属性、事件、字段等，含各成员 uid。返回的成员 uid 可直接用于 get_api（看签名文档）或 get_source（看实现源码）。")]
    public string GetTypeMembers(
        [Description("类型的 docfx uid，如 \"Kook.IGuild\"。")]
        string typeUid)
    {
        ApiItem? type = _api.GetByUid(typeUid);
        if (type is null)
            return $"未找到 uid 为 \"{typeUid}\" 的类型。";

        IReadOnlyList<ApiItem> members = _api.GetMembers(typeUid);
        if (members.Count == 0)
            return $"类型 {type.FullName ?? type.Uid} 没有可列出的成员。";

        StringBuilder sb = new();
        sb.AppendLine($"{type.FullName ?? type.Uid} 的成员（{members.Count} 个）：");
        sb.AppendLine();
        foreach (IGrouping<string, ApiItem> group in members
            .GroupBy(m => m.Type ?? "Other")
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
        {
            sb.AppendLine($"## {group.Key}");
            foreach (ApiItem m in group.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase))
            {
                sb.AppendLine($"- {m.Name}");
                sb.AppendLine($"  uid: {m.Uid}");
                if (!string.IsNullOrWhiteSpace(m.Summary))
                    sb.AppendLine($"  摘要：{Clean(m.Summary)}");
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    private string Format(ApiItem item, bool includeMembers)
    {
        StringBuilder sb = new();
        sb.AppendLine($"# {item.FullName ?? item.Name}  [{item.Type}]");
        sb.AppendLine($"uid: {item.Uid}");
        if (!string.IsNullOrWhiteSpace(item.Namespace))
            sb.AppendLine($"命名空间：{item.Namespace}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(item.Syntax?.Content))
        {
            sb.AppendLine("签名：");
            sb.AppendLine("```csharp");
            sb.AppendLine(item.Syntax.Content);
            sb.AppendLine("```");
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(item.Summary))
        {
            sb.AppendLine("摘要：");
            sb.AppendLine(Clean(item.Summary));
            sb.AppendLine();
        }

        if (item.Syntax?.Parameters is { Count: > 0 } parameters)
        {
            sb.AppendLine("参数：");
            foreach (ApiParameter p in parameters)
                sb.AppendLine($"- {p.Id} ({p.Type}): {Clean(p.Description)}");
            sb.AppendLine();
        }

        if (item.Syntax?.Return is { } ret && (ret.Type is not null || ret.Description is not null))
        {
            sb.AppendLine($"返回值：{ret.Type} — {Clean(ret.Description)}");
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(item.Remarks))
        {
            sb.AppendLine("备注：");
            sb.AppendLine(Clean(item.Remarks));
            sb.AppendLine();
        }

        if (includeMembers && item.Children is { Count: > 0 } children)
        {
            sb.AppendLine($"成员（{children.Count} 个，用 get_type_members 展开详情）：");
            foreach (string childUid in children.Take(50))
                sb.AppendLine($"- {childUid}");
            if (children.Count > 50)
                sb.AppendLine($"…以及其余 {children.Count - 50} 个。");
        }

        return sb.ToString().TrimEnd();
    }

    private static string Clean(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // docfx 的 summary/remarks 已渲染为含 HTML 标签的文本，去标签取纯文本。
        StringBuilder sb = new(html.Length);
        bool inTag = false;
        foreach (char c in html)
        {
            if (c == '<') inTag = true;
            else if (c == '>') inTag = false;
            else if (!inTag) sb.Append(c);
        }

        return sb.ToString().Replace('\n', ' ').Replace("  ", " ").Trim();
    }
}
