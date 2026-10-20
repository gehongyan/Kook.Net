using System.Text.Json;

namespace Kook.Net.Docs.Mcp.Search;

/// <summary>
///     读取 docfx <c>ExtractSearchIndex</c> 后处理器生成的 <c>_site/index.json</c>，
///     并基于标题与关键字做简单打分检索。
/// </summary>
/// <remarks>
///     <c>index.json</c> 的结构是「href -&gt; { title, keywords }」的字典。
///     索引在首次使用时惰性加载并缓存，文件修改后自动重载。
/// </remarks>
public sealed class SearchIndexService
{
    private readonly DocsPaths _paths;
    private readonly object _gate = new();
    private Dictionary<string, SearchIndexEntry>? _index;
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

        Dictionary<string, SearchIndexEntry> index = EnsureLoaded();
        string[] terms = query
            .Split([' ', '\t', '\r', '\n', '.', ',', '(', ')', '<', '>'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(t => t.ToLowerInvariant())
            .Distinct()
            .ToArray();

        if (terms.Length == 0)
            return [];

        List<SearchResult> results = [];
        foreach ((string href, SearchIndexEntry entry) in index)
        {
            double score = Score(entry, terms, out string? snippet);
            if (score > 0)
                results.Add(new SearchResult(entry.Title ?? href, href, score, snippet));
        }

        return results
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Title, StringComparer.OrdinalIgnoreCase)
            .Take(Math.Max(1, limit))
            .ToList();
    }

    private static double Score(SearchIndexEntry entry, string[] terms, out string? snippet)
    {
        snippet = null;
        string title = entry.Title ?? string.Empty;
        string titleLower = title.ToLowerInvariant();
        string keywords = entry.Keywords ?? string.Empty;
        string keywordsLower = keywords.ToLowerInvariant();

        double score = 0;
        foreach (string term in terms)
        {
            // 标题命中权重最高；标题完全等于查询词再加成。
            if (titleLower.Contains(term))
            {
                score += 10;
                if (titleLower == term)
                    score += 10;
            }

            if (keywordsLower.Contains(term))
            {
                score += 1;
                snippet ??= ExtractSnippet(keywords, term);
            }
        }

        return score;
    }

    private static string? ExtractSnippet(string keywords, string term)
    {
        int idx = keywords.IndexOf(term, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return null;

        int start = Math.Max(0, idx - 40);
        int end = Math.Min(keywords.Length, idx + term.Length + 40);
        string fragment = keywords[start..end].Replace('\n', ' ').Trim();
        return (start > 0 ? "…" : string.Empty)
            + fragment
            + (end < keywords.Length ? "…" : string.Empty);
    }

    private Dictionary<string, SearchIndexEntry> EnsureLoaded()
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
            if (_index is not null && writeTime == _loadedWriteTimeUtc)
                return _index;

            using FileStream stream = File.OpenRead(_paths.SearchIndexFile);
            Dictionary<string, SearchIndexEntry>? parsed =
                JsonSerializer.Deserialize(stream, DocsJsonContext.Default.DictionaryStringSearchIndexEntry);

            _index = parsed ?? [];
            _loadedWriteTimeUtc = writeTime;
            return _index;
        }
    }
}
