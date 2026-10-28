using Kook.Net.Docs.Mcp.Api;

namespace Kook.Net.Docs.Mcp.Source;

/// <summary>
///     按 docfx uid 读取类型 / 成员的源码片段。
/// </summary>
/// <remarks>
///     定位依据是 docfx ManagedReference 条目的 <c>source</c> 块（源码文件 + 起始行）。
///     docfx 不记录结束行，本服务按「同一文件内下一个条目的起始行」推断结束边界，
///     并以最大行数上限兜底。所有路径读取都做 docfx 工程根的目录包含校验，
///     拒绝越界读取工程之外的文件。
/// </remarks>
public sealed class SourceService
{
    /// <summary>单次返回源码片段的硬上限行数，任何请求都不会超过此值（失控防护，而非内容闸门）。</summary>
    private const int MaxLinesHardCap = 5000;

    /// <summary>未显式指定 <c>maxLines</c> 且未请求完整块时的默认行数上限。</summary>
    private const int DefaultMaxLines = 60;

    /// <summary>被视为「类型」的 docfx 条目类型：返回整个类型块而非到首个成员为止。</summary>
    private static readonly HashSet<string> TypeKinds =
        new(StringComparer.OrdinalIgnoreCase) { "Class", "Struct", "Enum", "Interface", "Delegate", "Record" };

    private readonly DocsPaths _paths;
    private readonly ApiModelService _api;

    /// <summary>
    ///     构造源码服务，注入路径解析器与 API 模型服务。
    /// </summary>
    public SourceService(DocsPaths paths, ApiModelService api)
    {
        _paths = paths;
        _api = api;
    }

    /// <summary>
    ///     按 uid 读取源码片段；uid 不存在、缺少源码位置或文件不可读时返回 <see langword="null" />。
    /// </summary>
    /// <param name="uid">目标 API 的 docfx uid。</param>
    /// <param name="maxLines">
    ///     返回片段的最大行数；为 <see langword="null" /> 时用默认值 <see cref="DefaultMaxLines" />。
    ///     任何值都会被收敛到 1..<see cref="MaxLinesHardCap" /> 区间。超出上限则截断并标记 <c>Truncated</c>。
    /// </param>
    /// <param name="fullBody">
    ///     为 <see langword="true" /> 时忽略 <paramref name="maxLines" /> 与默认值，按硬上限
    ///     <see cref="MaxLinesHardCap" /> 返回尽可能完整的块，用于下游明确需要完整源码的场景。
    /// </param>
    public SourceSnippet? GetByUid(string uid, int? maxLines = null, bool fullBody = false)
    {
        ApiItem? item = _api.GetByUid(uid);
        if (item?.Source?.Path is not { Length: > 0 } relativePath)
            return null;

        // source.path 相对 docfx 工程根（docfx.json 所在目录），如 "../src/.../X.cs"。
        string fullPath = Path.GetFullPath(Path.Combine(_paths.Root, relativePath));

        // 目录包含校验：只允许读取工程根的父目录（仓库）范围内的源码，拒绝越界。
        string repoRoot = Path.GetFullPath(Path.Combine(_paths.Root, ".."));
        if (!IsWithin(fullPath, repoRoot) || !File.Exists(fullPath))
            return null;

        string[] lines = File.ReadAllLines(fullPath);

        // docfx 的 startLine 为 0 基，转成 1 基的数组下标参考。
        int declarationIndex = Math.Clamp(item.Source.StartLine, 0, Math.Max(0, lines.Length - 1));

        // docfx 的 startLine 指向声明行，不含其上方属于该成员自身的 XML 文档注释（及紧贴的特性）。
        // 这些注释是描述该 API 的关键信息，向上回溯把它们纳入片段；回溯不越过上一个同文件兄弟条目。
        int startIndex = BacktrackLeadingDocComment(item, relativePath, declarationIndex, lines);

        int endIndexExclusive = ResolveEndExclusive(item, relativePath, item.Source.StartLine, lines);

        // fullBody 直接用硬上限；否则用显式 maxLines 或默认值，并一律收敛到 1..硬上限。
        int requested = fullBody ? MaxLinesHardCap : (maxLines ?? DefaultMaxLines);
        int effectiveMax = Math.Clamp(requested, 1, MaxLinesHardCap);
        bool truncated = false;
        if (endIndexExclusive - startIndex > effectiveMax)
        {
            endIndexExclusive = startIndex + effectiveMax;
            truncated = true;
        }

        string code = string.Join('\n', lines[startIndex..endIndexExclusive]);

        return new SourceSnippet
        {
            Uid = item.Uid,
            RepositoryPath = item.Source.Remote?.Path ?? relativePath,
            StartLine = startIndex + 1,
            EndLine = endIndexExclusive,
            Code = code,
            Url = BuildUrl(item.Source.Remote, item.Source.StartLine),
            Truncated = truncated,
            PartialOtherFiles = ResolvePartialOtherFiles(item, relativePath)
        };
    }

    /// <summary>
    ///     检测跨文件 partial 类型：若目标是类型，且其成员中存在 <c>source.path</c> 与目标自身所在文件不同者，
    ///     则返回这些「其余文件」（仓库相对路径，去重且不含本文件，按序）。
    ///     docfx 的类型条目只记录单个 partial 声明的位置，故此时本片段可能只覆盖该类型的一部分。
    ///     非类型目标、或所有成员都与目标同文件时，返回空列表。
    /// </summary>
    private IReadOnlyList<string> ResolvePartialOtherFiles(ApiItem target, string targetRelativePath)
    {
        if (target.Type is not { Length: > 0 } t || !TypeKinds.Contains(t))
            return [];

        if (target.Children is not { Count: > 0 } children)
            return [];

        SortedSet<string> otherFiles = new(StringComparer.OrdinalIgnoreCase);
        foreach (string childUid in children)
        {
            string? childPath = _api.GetByUid(childUid)?.Source?.Path;
            if (childPath is { Length: > 0 }
                && !string.Equals(childPath, targetRelativePath, StringComparison.OrdinalIgnoreCase))
            {
                otherFiles.Add(childPath);
            }
        }

        return otherFiles.Count == 0 ? [] : otherFiles.ToList();
    }

    /// <summary>
    ///     推断结束边界（0 基、独占）。
    /// </summary>
    /// <remarks>
    ///     目标是「类型」（class / record / struct / enum / interface / delegate）时，返回整个类型块：
    ///     跳过该类型自身的全部成员（含嵌套类型及其成员，按 <c>parent</c> 链判定为后代），
    ///     边界取同一源码文件内「起始行大于目标且不属于该类型后代」的最小条目起始行。
    ///     目标是「成员」时，沿用原逻辑：取同一文件内起始行严格大于目标的最小条目起始行。
    ///     没有这样的后继条目时读到文件末尾。
    /// </remarks>
    private int ResolveEndExclusive(ApiItem target, string relativePath, int startLine, string[] lines)
    {
        int fileLineCount = lines.Length;
        bool targetIsType = target.Type is { Length: > 0 } t && TypeKinds.Contains(t);

        int next = int.MaxValue;
        foreach (ApiItem other in _api.GetAll())
        {
            ApiSource? src = other.Source;
            if (src?.Path is null || !string.Equals(src.Path, relativePath, StringComparison.OrdinalIgnoreCase))
                continue;

            if (src.StartLine <= startLine || src.StartLine >= next)
                continue;

            // 类型目标：忽略其后代条目（成员、嵌套类型及嵌套成员），使块覆盖整个类型。
            if (targetIsType && IsDescendantOf(other, target.Uid))
                continue;

            next = src.StartLine;
        }

        if (next == int.MaxValue)
            return fileLineCount;

        next = Math.Min(next, fileLineCount);

        // docfx 的 startLine 指向声明行，不含其上方的 XML 文档注释；若直接以后继条目的
        // 声明行为边界，会把后继成员的前导 /// 文档注释尾随进当前片段。从后继条目声明行
        // 向上回退紧邻的连续 /// 行（可含夹在其间的空行），使其归属后继成员而非当前片段。
        return TrimTrailingDocComment(lines, next, startLine);
    }

    /// <summary>
    ///     把结束边界 <paramref name="endExclusive" />（0 基、独占，指向后继条目声明行）向上回退，
    ///     剥离紧邻其上、连续的 XML 文档注释块（<c>///</c>，允许其间夹空行），使这些注释归属后继成员。
    ///     回退不会越过 <paramref name="lowerBound" />（当前目标的起始行），且遇到首个非 <c>///</c>、
    ///     非空行即停。无前导文档注释时原样返回。
    /// </summary>
    private static int TrimTrailingDocComment(string[] lines, int endExclusive, int lowerBound)
    {
        int cut = endExclusive;
        int firstDoc = -1;

        // 从后继声明行的上一行向上扫，越过连续的 /// 与夹在其间的空行。
        for (int i = endExclusive - 1; i > lowerBound; i--)
        {
            string trimmed = lines[i].Trim();
            if (trimmed.StartsWith("///", StringComparison.Ordinal))
            {
                firstDoc = i;
                continue;
            }

            // 允许 /// 注释块内部的空行；但仅当其上方仍是 /// 时才继续。
            if (trimmed.Length == 0 && firstDoc >= 0)
                continue;

            break;
        }

        // 仅当确实找到紧贴后继声明的前导 /// 块时才回退，回退到该块首行。
        if (firstDoc >= 0)
            cut = firstDoc;

        return cut;
    }

    /// <summary>
    ///     把起始边界 <paramref name="declarationIndex" />（0 基，指向成员声明行）向上回溯，
    ///     纳入紧邻其上、属于该成员自身的 XML 文档注释（<c>///</c>）及紧贴声明的特性行（<c>[...]</c>），
    ///     允许其间夹空行。回溯不会越过上一个同文件兄弟条目的声明行，且遇到首个不属于前导块的行
    ///     （普通代码、上一成员的结束括号等）即停。无前导文档注释 / 特性时原样返回声明行下标。
    /// </summary>
    private int BacktrackLeadingDocComment(ApiItem target, string relativePath, int declarationIndex, string[] lines)
    {
        // 下界：同一文件内起始行严格小于目标声明行的最大条目起始行（上一个兄弟声明行）。
        // 回溯不得越过它，避免把上一个成员的尾随内容错纳入本片段。前导文档注释永远位于
        // 目标声明行与上一兄弟声明行之间，故此下界不会误伤目标自身的注释。
        int lowerBound = -1;
        foreach (ApiItem other in _api.GetAll())
        {
            ApiSource? src = other.Source;
            if (src?.Path is null || !string.Equals(src.Path, relativePath, StringComparison.OrdinalIgnoreCase))
                continue;
            if (src.StartLine < declarationIndex && src.StartLine > lowerBound)
                lowerBound = src.StartLine;
        }

        int start = declarationIndex;
        int firstLead = -1;
        bool seenLead = false;

        // 从声明行上一行向上扫，越过连续的 /// 文档注释、紧贴声明的特性行，以及夹在其间的空行。
        for (int i = declarationIndex - 1; i > lowerBound; i--)
        {
            string trimmed = lines[i].Trim();

            if (trimmed.StartsWith("///", StringComparison.Ordinal)
                || (trimmed.StartsWith('[') && trimmed.EndsWith(']')))
            {
                firstLead = i;
                seenLead = true;
                continue;
            }

            // 允许前导块内部的空行；仅当已进入前导块时才跨越。
            if (trimmed.Length == 0 && seenLead)
                continue;

            break;
        }

        if (firstLead >= 0)
            start = firstLead;

        return Math.Clamp(start, 0, Math.Max(0, lines.Length - 1));
    }

    /// <summary>
    ///     判断 <paramref name="item" /> 是否为 uid 为 <paramref name="ancestorUid" /> 的条目的后代
    ///     （沿 <c>parent</c> 链向上可达）。用于把类型自身的成员与嵌套类型排除出结束边界计算。
    /// </summary>
    private bool IsDescendantOf(ApiItem item, string ancestorUid)
    {
        // 防御性限制遍历深度，避免异常数据造成的环。
        string? parent = item.Parent;
        for (int depth = 0; parent is { Length: > 0 } && depth < 64; depth++)
        {
            if (string.Equals(parent, ancestorUid, StringComparison.Ordinal))
                return true;
            parent = _api.GetByUid(parent)?.Parent;
        }

        return false;
    }

    /// <summary>
    ///     按仓库相对路径读取一个 <c>.cs</c> 源码文件（可选行范围）。
    ///     路径非法、越界、非 <c>.cs</c> 或文件不存在时返回 <see langword="null" />。
    /// </summary>
    /// <param name="repositoryPath">
    ///     仓库相对路径，如 <c>src/Kook.Net.WebSocket/.../SocketGuild.Events.cs</c>，
    ///     即 <c>get_source</c> 对 partial 类型返回的「其余文件」路径。
    /// </param>
    /// <param name="startLine">起始行号（1 基，含）；为 <see langword="null" /> 时从第 1 行开始。</param>
    /// <param name="maxLines">
    ///     最大返回行数；为 <see langword="null" /> 时用默认值 <see cref="DefaultMaxLines" />。
    ///     收敛到 1..<see cref="MaxLinesHardCap" />。超出则截断并标记 <c>Truncated</c>。
    /// </param>
    public SourceFileResult? ReadFile(string repositoryPath, int? startLine = null, int? maxLines = null)
    {
        if (string.IsNullOrWhiteSpace(repositoryPath))
            return null;

        // 只允许 .cs，拒绝其它文件类型。
        if (!repositoryPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
            return null;

        // 仓库相对路径以 docfx 工程根的父目录（仓库根）为基准解析。
        string repoRoot = Path.GetFullPath(Path.Combine(_paths.Root, ".."));
        string fullPath = Path.GetFullPath(Path.Combine(repoRoot, repositoryPath));

        // 目录包含校验：拒绝 ../ 越界读取仓库之外的文件。
        if (!IsWithin(fullPath, repoRoot) || !File.Exists(fullPath))
            return null;

        string[] lines = File.ReadAllLines(fullPath);
        int total = lines.Length;
        if (total == 0)
        {
            return new SourceFileResult
            {
                RepositoryPath = repositoryPath,
                StartLine = 0,
                EndLine = 0,
                TotalLines = 0,
                Code = string.Empty,
                Truncated = false
            };
        }

        // 1 基入参转 0 基数组下标。
        int startIndex = Math.Clamp((startLine ?? 1) - 1, 0, total - 1);
        int effectiveMax = Math.Clamp(maxLines ?? DefaultMaxLines, 1, MaxLinesHardCap);

        int endIndexExclusive = startIndex + effectiveMax;
        bool truncated = false;
        if (endIndexExclusive < total)
            truncated = true;
        else
            endIndexExclusive = total;

        string code = string.Join('\n', lines[startIndex..endIndexExclusive]);

        return new SourceFileResult
        {
            RepositoryPath = repositoryPath,
            StartLine = startIndex + 1,
            EndLine = endIndexExclusive,
            TotalLines = total,
            Code = code,
            Truncated = truncated
        };
    }

    private static string? BuildUrl(ApiSourceRemote? remote, int startLine)
    {
        if (remote?.Repo is not { Length: > 0 } repo || remote.Path is not { Length: > 0 } path)
            return null;

        string baseUrl = repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase)
            ? repo[..^4]
            : repo;
        string branch = string.IsNullOrWhiteSpace(remote.Branch) ? "HEAD" : remote.Branch;

        // startLine 为 0 基，GitHub 行锚点为 1 基。
        return $"{baseUrl}/blob/{branch}/{path}#L{startLine + 1}";
    }

    private static bool IsWithin(string candidate, string root)
    {
        string normalizedRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        string normalizedCandidate = Path.GetFullPath(candidate);
        return normalizedCandidate.StartsWith(normalizedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalizedCandidate, normalizedRoot, StringComparison.OrdinalIgnoreCase);
    }
}
