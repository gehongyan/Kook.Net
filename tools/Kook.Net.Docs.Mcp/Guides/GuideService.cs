namespace Kook.Net.Docs.Mcp.Guides;

/// <summary>
///     列举与读取 docfx 工程下的概念 / 指南类 Markdown 文档
///     （<c>guides</c>、<c>quick_reference</c>、<c>demos</c>、<c>changelog</c>）。
/// </summary>
/// <remarks>
///     读取时对传入路径做根目录包含性校验，拒绝越出工程根目录的路径（防目录穿越）。
/// </remarks>
public sealed class GuideService
{
    private static readonly string[] Categories =
        ["guides", "quick_reference", "demos", "changelog"];

    private readonly DocsPaths _paths;

    /// <summary>
    ///     使用给定的路径解析器构造指南服务。
    /// </summary>
    public GuideService(DocsPaths paths) => _paths = paths;

    /// <summary>
    ///     列出所有 Markdown 文档，可选按分组过滤。
    /// </summary>
    /// <param name="category">分组过滤（<c>guides</c> 等）；为空列出全部分组。</param>
    public IReadOnlyList<GuideDocument> List(string? category = null)
    {
        List<GuideDocument> docs = [];
        foreach (string cat in Categories)
        {
            if (!string.IsNullOrWhiteSpace(category)
                && !string.Equals(cat, category, StringComparison.OrdinalIgnoreCase))
                continue;

            string dir = Path.Combine(_paths.Root, cat);
            if (!Directory.Exists(dir))
                continue;

            foreach (string file in Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories))
            {
                string relative = Path.GetRelativePath(_paths.Root, file).Replace('\\', '/');
                docs.Add(new GuideDocument(relative, cat, ReadTitle(file)));
            }
        }

        return docs
            .OrderBy(d => d.Category, StringComparer.OrdinalIgnoreCase)
            .ThenBy(d => d.RelativePath, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    ///     读取一篇 Markdown 文档的原始内容。
    /// </summary>
    /// <param name="relativePath">相对于工程根目录的路径（<see cref="GuideDocument.RelativePath" />）。</param>
    /// <exception cref="ArgumentException">路径越出工程根目录，或不是 <c>.md</c> 文件。</exception>
    /// <exception cref="FileNotFoundException">文件不存在。</exception>
    public string Read(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            throw new ArgumentException("路径不能为空。", nameof(relativePath));

        string full = Path.GetFullPath(Path.Combine(_paths.Root, relativePath));
        string rootWithSep = _paths.Root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;

        if (!full.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("路径越出文档工程根目录。", nameof(relativePath));

        if (!full.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("只能读取 .md 文档。", nameof(relativePath));

        if (!File.Exists(full))
            throw new FileNotFoundException($"未找到文档 {relativePath}。", full);

        return File.ReadAllText(full);
    }

    private static string ReadTitle(string file)
    {
        try
        {
            foreach (string line in File.ReadLines(file))
            {
                string trimmed = line.TrimStart();
                if (trimmed.StartsWith("# ", StringComparison.Ordinal))
                    return trimmed[2..].Trim();
            }
        }
        catch
        {
            // 读取失败时退回文件名。
        }

        return Path.GetFileNameWithoutExtension(file);
    }
}
