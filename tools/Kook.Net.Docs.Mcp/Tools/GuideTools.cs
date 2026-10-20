using System.ComponentModel;
using System.Text;
using Kook.Net.Docs.Mcp.Guides;
using ModelContextProtocol.Server;

namespace Kook.Net.Docs.Mcp.Tools;

/// <summary>
///     概念 / 指南类 Markdown 文档相关的 MCP 工具。
/// </summary>
[McpServerToolType]
public sealed class GuideTools
{
    private readonly GuideService _guides;

    /// <summary>
    ///     构造指南工具，注入指南服务。
    /// </summary>
    public GuideTools(GuideService guides) => _guides = guides;

    /// <summary>
    ///     列出指南 / 速查 / 示例 / 更新日志类文档。
    /// </summary>
    [McpServerTool(Name = "list_guides")]
    [Description("列出概念文档：指南(guides)、速查手册(quick_reference)、示例说明(demos)、更新日志(changelog)，返回各文档的标题与路径。")]
    public string ListGuides(
        [Description("可选分组过滤：guides / quick_reference / demos / changelog；为空列出全部。")]
        string? category = null)
    {
        IReadOnlyList<GuideDocument> docs = _guides.List(category);
        if (docs.Count == 0)
            return category is null ? "没有找到任何文档。" : $"分组 \"{category}\" 下没有找到文档。";

        StringBuilder sb = new();
        sb.AppendLine($"共 {docs.Count} 篇文档：");
        sb.AppendLine();
        foreach (IGrouping<string, GuideDocument> group in docs
            .GroupBy(d => d.Category)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
        {
            sb.AppendLine($"## {group.Key}");
            foreach (GuideDocument d in group)
            {
                sb.AppendLine($"- {d.Title}");
                sb.AppendLine($"  路径：{d.RelativePath}");
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    ///     读取一篇 Markdown 文档的原始内容。
    /// </summary>
    [McpServerTool(Name = "read_guide")]
    [Description("读取一篇 Markdown 文档的原始内容。path 用 list_guides 返回的相对路径。")]
    public string ReadGuide(
        [Description("文档相对路径，如 \"guides/getting_started/first-bot.md\"。")]
        string path)
    {
        try
        {
            return _guides.Read(path);
        }
        catch (Exception ex) when (ex is ArgumentException or FileNotFoundException)
        {
            return $"读取失败：{ex.Message}";
        }
    }
}
