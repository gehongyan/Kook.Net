using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Kook.Net.Docs.Mcp.Search;

/// <summary>
///     文档全文检索服务。
/// </summary>
/// <remarks>
///     docfx 的 <c>ExtractSearchIndex</c> 后处理器在当前版本下只生成 <c>href -&gt; { title }</c>，
///     <c>keywords</c> 字段为空，不足以支撑正文检索。因此本服务以 <c>_site/index.json</c>
///     的 <c>href</c> / <c>title</c> 列表为锚，再从对应的 <c>_site/&lt;href&gt;</c> 静态 HTML 页面
///     提取 <c>&lt;article&gt;</c> 正文纯文本，建立内存全文索引。
///     索引在首次使用时惰性加载，并随 <c>index.json</c> 的修改时间失效重建
///     （HTML 与 index.json 同一次 docfx 构建产出，故以后者的写时间为缓存键）。
/// </remarks>
public sealed class SearchIndexService
{
    private readonly DocsPaths _paths;
    private readonly object _gate = new();
    private List<IndexedPage>? _pages;
    private DateTime _loadedWriteTimeUtc;

    /// <summary>
    ///     使用给定的路径解析器构造检索服务。
    /// </summary>
    public SearchIndexService(DocsPaths paths) => _paths = paths;

    /// <summary>
    ///     检索文档。按与查询词的匹配程度降序返回至多 <paramref name="limit" /> 条结果。
    /// </summary>
    /// <param name="query">查询词，空白时返回空结果。</param>
    /// <param name="limit">最大返回条数。</param>
    public IReadOnlyList<SearchResult> Search(string query, int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        List<IndexedPage> pages = EnsureLoaded();
        string[] terms = Tokenize(query);
        if (terms.Length == 0)
            return [];

        List<SearchResult> results = [];
        foreach (IndexedPage page in pages)
        {
            double score = Score(page, terms, out string? snippet);
            if (score > 0)
                results.Add(new SearchResult(page.Title, page.Href, score, snippet));
        }

        return results
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Title, StringComparer.OrdinalIgnoreCase)
            .Take(Math.Max(1, limit))
            .ToList();
    }

    /// <summary>
    ///     将查询串切分为检索词：按空白与常见标点分词并小写化去重。
    ///     中文整串不含空白时作为单个词保留，依靠对正文做子串匹配命中（正文为连续原文）。
    /// </summary>
    private static string[] Tokenize(string text) =>
        text
            .Split(
                [' ', '\t', '\r', '\n', '.', ',', '，', '。', '、', '(', ')', '（', '）', '<', '>', '【', '】', '[', ']', ':', '：', '/'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(t => t.ToLowerInvariant())
            .Distinct()
            .ToArray();

    private static double Score(IndexedPage page, string[] terms, out string? snippet)
    {
        snippet = null;
        double score = 0;
        foreach (string term in terms)
        {
            // 标题命中权重最高；标题完全等于查询词再加成。
            if (page.TitleLower.Contains(term))
            {
                score += 10;
                if (page.TitleLower == term)
                    score += 10;
            }

            // 正文（从 HTML 提取）命中：计分并提取片段。
            if (page.ContentLower.Contains(term))
            {
                score += 2;
                snippet ??= ExtractSnippet(page.Content, term);
            }
        }

        return score;
    }

    private static string? ExtractSnippet(string content, string term)
    {
        int idx = content.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return null;

        int start = Math.Max(0, idx - 40);
        int end = Math.Min(content.Length, idx + term.Length + 40);
        string fragment = content[start..end].Replace('\n', ' ').Trim();
        return (start > 0 ? "…" : string.Empty)
            + fragment
            + (end < content.Length ? "…" : string.Empty);
    }

    private List<IndexedPage> EnsureLoaded()
    {
        lock (_gate)
        {
            if (!File.Exists(_paths.SearchIndexFile))
            {
                throw new FileNotFoundException(
                    $"未找到检索索引 {_paths.SearchIndexFile}。请先运行 `docfx build` " +
                    $"（或 `docfx {_paths.Root}/docfx.json`）生成 _site/index.json。",
                    _paths.SearchIndexFile);
            }

            DateTime writeTime = File.GetLastWriteTimeUtc(_paths.SearchIndexFile);
            if (_pages is not null && writeTime == _loadedWriteTimeUtc)
                return _pages;

            _pages = BuildIndex();
            _loadedWriteTimeUtc = writeTime;
            return _pages;
        }
    }

    /// <summary>
    ///     读取 <c>index.json</c> 的 href/title 列表，并从各自对应的 <c>_site</c> HTML 页面
    ///     提取正文纯文本，构建内存全文索引。无法读取或解析的页面以标题占位，仍可按标题检索。
    /// </summary>
    private List<IndexedPage> BuildIndex()
    {
        using FileStream stream = File.OpenRead(_paths.SearchIndexFile);
        Dictionary<string, SearchIndexEntry>? parsed =
            JsonSerializer.Deserialize(stream, DocsJsonContext.Default.DictionaryStringSearchIndexEntry);

        List<IndexedPage> pages = [];
        if (parsed is null)
            return pages;

        foreach ((string href, SearchIndexEntry entry) in parsed)
        {
            string title = entry.Title ?? href;
            string content = ExtractPageContent(href);
            pages.Add(new IndexedPage(href, title, content));
        }

        return pages;
    }

    /// <summary>
    ///     从 <c>_site/&lt;href&gt;</c> 的静态 HTML 读取并提取 <c>&lt;article&gt;</c> 正文纯文本。
    ///     文件不存在或读取失败时返回空串（该页退化为仅按标题检索）。
    /// </summary>
    private string ExtractPageContent(string href)
    {
        string htmlPath = Path.GetFullPath(Path.Combine(_paths.SiteDirectory, href));

        // 目录包含校验：只读取 _site 目录内的文件。
        string site = Path.TrimEndingDirectorySeparator(Path.GetFullPath(_paths.SiteDirectory));
        if (!htmlPath.StartsWith(site + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        if (!File.Exists(htmlPath))
            return string.Empty;

        try
        {
            string html = File.ReadAllText(htmlPath);
            return HtmlToText(html);
        }
        catch (IOException)
        {
            return string.Empty;
        }
        catch (UnauthorizedAccessException)
        {
            return string.Empty;
        }
    }

    private static readonly Regex ArticleRegex =
        new(@"<article\b[^>]*>(.*?)</article>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ScriptStyleRegex =
        new(@"<(script|style)\b[^>]*>.*?</\1>",
            RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex TagRegex =
        new(@"<[^>]+>", RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex WhitespaceRegex =
        new(@"\s+", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <summary>
    ///     将一段 HTML 转为检索用纯文本：优先取 <c>&lt;article&gt;</c> 正文，剥离脚本/样式与所有标签，
    ///     解码 HTML 实体并规整空白。无 <c>&lt;article&gt;</c> 时退化为对整页做同样处理。
    /// </summary>
    private static string HtmlToText(string html)
    {
        Match article = ArticleRegex.Match(html);
        string body = article.Success ? article.Groups[1].Value : html;

        body = ScriptStyleRegex.Replace(body, " ");
        body = TagRegex.Replace(body, " ");
        body = WebUtility.HtmlDecode(body);
        body = WhitespaceRegex.Replace(body, " ").Trim();
        return body;
    }

    /// <summary>
    ///     内存中的单页索引记录：链接、标题、从 HTML 提取的正文，及二者的小写缓存。
    /// </summary>
    private sealed class IndexedPage
    {
        public IndexedPage(string href, string title, string content)
        {
            Href = href;
            Title = title;
            Content = content;
            TitleLower = title.ToLowerInvariant();
            ContentLower = content.ToLowerInvariant();
        }

        public string Href { get; }
        public string Title { get; }
        public string Content { get; }
        public string TitleLower { get; }
        public string ContentLower { get; }
    }
}
