using System.Diagnostics.CodeAnalysis;

namespace Kook.Net.Docs.Mcp;

/// <summary>
///     解析 docfx 文档工程及其构建产物的磁盘路径。
/// </summary>
/// <remarks>
///     定位优先级：
///     <list type="number">
///         <item>环境变量 <c>KOOK_DOCS_ROOT</c>（指向包含 <c>docfx.json</c> 的目录）；</item>
///         <item>从当前工作目录逐级向上查找 <c>docfx.json</c>；</item>
///         <item>从程序集所在目录逐级向上查找 <c>docfx.json</c>。</item>
///     </list>
///     本服务为纯路径解析，不读取文件内容，也不校验产物是否已生成。
/// </remarks>
public sealed class DocsPaths
{
    /// <summary>
    ///     指向 docfx 工程根目录的环境变量名。
    /// </summary>
    public const string RootEnvironmentVariable = "KOOK_DOCS_ROOT";

    /// <summary>
    ///     指定 API ManagedReference YAML 子目录名的环境变量名。
    /// </summary>
    /// <remarks>
    ///     默认 <c>api</c>（公开 API 产物）。MCP 部署若使用含 internal / private 成员的
    ///     细化产物，可设为 <c>api-mcp</c>（由 <c>docfx metadata docs/docfx.mcp.json</c> 生成）。
    /// </remarks>
    public const string ApiDirectoryEnvironmentVariable = "KOOK_DOCS_API_DIR";

    private DocsPaths(string root)
    {
        Root = root;

        string apiDirName = Environment.GetEnvironmentVariable(ApiDirectoryEnvironmentVariable) is { Length: > 0 } configured
            ? configured
            : "api";

        ApiDirectory = Path.Combine(root, apiDirName);
        GuidesDirectory = Path.Combine(root, "guides");
        QuickReferenceDirectory = Path.Combine(root, "quick_reference");
        DemosDirectory = Path.Combine(root, "demos");
        ChangelogDirectory = Path.Combine(root, "changelog");
        SiteDirectory = Path.Combine(root, "_site");
        SearchIndexFile = Path.Combine(SiteDirectory, "index.json");
    }

    /// <summary>docfx 工程根目录（含 <c>docfx.json</c>）。</summary>
    public string Root { get; }

    /// <summary>
    ///     <c>docfx metadata</c> 生成的 API ManagedReference YAML 目录。
    ///     子目录名默认 <c>api</c>，可由环境变量 <see cref="ApiDirectoryEnvironmentVariable" /> 覆盖。
    /// </summary>
    public string ApiDirectory { get; }

    /// <summary>概念指南 Markdown 目录。</summary>
    public string GuidesDirectory { get; }

    /// <summary>速查手册 Markdown 目录。</summary>
    public string QuickReferenceDirectory { get; }

    /// <summary>示例说明 Markdown 目录。</summary>
    public string DemosDirectory { get; }

    /// <summary>更新日志 Markdown 目录。</summary>
    public string ChangelogDirectory { get; }

    /// <summary><c>docfx build</c> 生成的静态站点目录。</summary>
    public string SiteDirectory { get; }

    /// <summary>ExtractSearchIndex 后处理器生成的全文检索索引文件。</summary>
    public string SearchIndexFile { get; }

    /// <summary>
    ///     解析 docfx 工程根目录。解析不到时抛出 <see cref="DirectoryNotFoundException" />。
    /// </summary>
    public static DocsPaths Resolve()
    {
        if (TryResolve(out DocsPaths? paths))
            return paths;

        throw new DirectoryNotFoundException(
            $"未能定位 docfx 工程（未找到 docfx.json）。请设置环境变量 {RootEnvironmentVariable} " +
            "指向包含 docfx.json 的目录，或在该目录及其子目录内启动本服务。");
    }

    /// <summary>
    ///     尝试解析 docfx 工程根目录。
    /// </summary>
    public static bool TryResolve([NotNullWhen(true)] out DocsPaths? paths)
    {
        string? fromEnv = Environment.GetEnvironmentVariable(RootEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(fromEnv)
            && File.Exists(Path.Combine(fromEnv, "docfx.json")))
        {
            paths = new DocsPaths(Path.GetFullPath(fromEnv));
            return true;
        }

        if (TryFindUpwards(Directory.GetCurrentDirectory(), out string? fromCwd))
        {
            paths = new DocsPaths(fromCwd);
            return true;
        }

        if (TryFindUpwards(AppContext.BaseDirectory, out string? fromBase))
        {
            paths = new DocsPaths(fromBase);
            return true;
        }

        paths = null;
        return false;
    }

    private static bool TryFindUpwards(string start, [NotNullWhen(true)] out string? root)
    {
        DirectoryInfo? dir = new(start);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "docfx.json")))
            {
                root = dir.FullName;
                return true;
            }

            dir = dir.Parent;
        }

        root = null;
        return false;
    }
}
