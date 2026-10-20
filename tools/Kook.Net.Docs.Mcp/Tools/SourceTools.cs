using System.ComponentModel;
using System.Text;
using Kook.Net.Docs.Mcp.Source;
using ModelContextProtocol.Server;

namespace Kook.Net.Docs.Mcp.Tools;

/// <summary>
///     源码查阅相关的 MCP 工具：按 docfx uid 返回类型 / 成员的源码片段，或按仓库相对路径读取源码文件。
/// </summary>
[McpServerToolType]
public sealed class SourceTools
{
    private readonly SourceService _source;

    /// <summary>
    ///     构造源码工具，注入源码服务。
    /// </summary>
    public SourceTools(SourceService source) => _source = source;

    /// <summary>
    ///     按 uid 读取类型或成员的源码片段。
    /// </summary>
    [McpServerTool(Name = "get_source")]
    [Description("按 docfx uid 返回类型或成员定义处的源码片段（含文件路径、行号与在线链接）。uid 可由 find_api / get_api / get_type_members 得到。当 uid 指向类型（class/record/struct/enum/interface/delegate）时返回整个类型块（含全部成员，如枚举的所有值）；指向成员时返回该成员定义。")]
    public string GetSource(
        [Description("API 的 docfx uid，如 \"Kook.AlphaColor\" 或 \"Kook.IGuild.GetUserAsync(System.UInt64,...)\"。")]
        string uid,
        [Description("返回片段的最大行数，省略时默认 60；上限 5000，超出则截断并标注。查询大类型想看更多时可调高。")]
        int? maxLines = null,
        [Description("为 true 时忽略 maxLines，按上限返回尽可能完整的块，用于明确需要完整源码的场景。")]
        bool fullBody = false)
    {
        SourceSnippet? snippet = _source.GetByUid(uid, maxLines, fullBody);
        if (snippet is null)
            return $"未找到 uid 为 \"{uid}\" 的源码位置。该 uid 可能不存在，或其条目未携带 source 信息"
                 + "（请确认 docfx metadata 已生成含 source 字段的 api/*.yml）。";

        StringBuilder sb = new();
        sb.AppendLine($"# {snippet.Uid}");
        sb.AppendLine($"文件：{snippet.RepositoryPath}");
        sb.AppendLine($"行号：{snippet.StartLine}–{snippet.EndLine}"
            + (snippet.Truncated ? "（已达最大行数上限，片段被截断；可提高 maxLines 或传 fullBody=true 获取更多）" : string.Empty));
        if (!string.IsNullOrWhiteSpace(snippet.Url))
            sb.AppendLine($"在线：{snippet.Url}");

        if (snippet.PartialOtherFiles.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("> 注意：这是跨文件的 partial 类型，本片段只是其中一部分，其余部分位于："); 
            foreach (string f in snippet.PartialOtherFiles)
                sb.AppendLine($">   - {f}");
            sb.AppendLine("> 可用 read_source_file 读取上述文件的完整源码（含无成员的声明片段）；"
                + "或用 get_type_members 列出全部成员后用 get_source 逐个获取。");
        }

        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine(snippet.Code);
        sb.AppendLine("```");

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    ///     按仓库相对路径读取一个源码文件（可选起始行与行数）。
    /// </summary>
    [McpServerTool(Name = "read_source_file")]
    [Description("按仓库相对路径读取一个 .cs 源码文件的内容（可选起始行与最大行数），用于查看任意源文件的完整内容，包括 using、命名空间、文件头注释，以及 get_source 片段之外的上下文。典型场景之一：读取 get_source 对 partial 类型提示的「其余文件」以获取完整定义。仅允许读取仓库 src 范围内的 .cs 文件。")]
    public string ReadSourceFile(
        [Description("仓库相对路径，如 \"src/Kook.Net.WebSocket/Entities/Guilds/SocketGuild.Events.cs\"。")]
        string path,
        [Description("起始行号（1 基，含），省略从第 1 行开始。")]
        int? startLine = null,
        [Description("最大返回行数，省略默认 60，上限 5000，超出则截断并标注。")]
        int? maxLines = null)
    {
        SourceFileResult? result = _source.ReadFile(path, startLine, maxLines);
        if (result is null)
            return $"无法读取 \"{path}\"。请确认它是仓库 src 范围内、以 .cs 结尾的相对路径（不含 ../ 越界），且文件存在。";

        StringBuilder sb = new();
        sb.AppendLine($"文件：{result.RepositoryPath}");
        sb.AppendLine($"行号：{result.StartLine}–{result.EndLine} / 共 {result.TotalLines} 行"
            + (result.Truncated ? "（已达最大行数上限，内容被截断；可提高 maxLines 或指定 startLine 继续读取）" : string.Empty));
        sb.AppendLine();
        sb.AppendLine("```csharp");
        sb.AppendLine(result.Code);
        sb.AppendLine("```");

        return sb.ToString().TrimEnd();
    }
}
