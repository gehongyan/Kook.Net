using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kook.Net.Docs.Mcp.Api;

/// <summary>
///     加载并索引 docfx <c>docfx metadata</c> 生成的 <c>api/*.yml</c> ManagedReference 模型，
///     提供按 uid 精确查询、按名称模糊查询、以及展开类型成员的能力。
/// </summary>
/// <remarks>
///     所有 YAML 在首次使用时惰性加载，并按 <c>api</c> 目录的最后写入时间缓存；
///     目录内容变化（重新生成文档）后自动重载。
/// </remarks>
public sealed class ApiModelService
{
    private readonly DocsPaths _paths;
    private readonly object _gate = new();
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private Dictionary<string, ApiItem>? _byUid;
    private DateTime _loadedWriteTimeUtc;

    /// <summary>
    ///     使用给定的路径解析器构造 API 模型服务。
    /// </summary>
    public ApiModelService(DocsPaths paths) => _paths = paths;

    /// <summary>
    ///     按 uid 精确获取一个 API 条目，不存在时返回 <see langword="null" />。
    /// </summary>
    public ApiItem? GetByUid(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
            return null;

        return EnsureLoaded().GetValueOrDefault(uid);
    }

    /// <summary>
    ///     获取当前已加载的全部 API 条目（按 uid 去重）。
    /// </summary>
    /// <remarks>
    ///     用于需要跨条目分析的场景，如按源码文件推断某成员定义的结束边界。
    /// </remarks>
    public IReadOnlyCollection<ApiItem> GetAll() => EnsureLoaded().Values;

    /// <summary>
    ///     按名称模糊查询 API 条目（匹配 uid / name / fullName，忽略大小写），
    ///     可选按条目类型过滤。
    /// </summary>
    /// <param name="query">查询词。</param>
    /// <param name="typeFilter">条目类型过滤，如 <c>Class</c>、<c>Method</c>；为空不过滤。</param>
    /// <param name="limit">最大返回条数。</param>
    public IReadOnlyList<ApiItem> Find(string query, string? typeFilter = null, int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        string q = query.Trim();
        IEnumerable<ApiItem> items = EnsureLoaded().Values;

        if (!string.IsNullOrWhiteSpace(typeFilter))
            items = items.Where(i => string.Equals(i.Type, typeFilter, StringComparison.OrdinalIgnoreCase));

        return items
            .Select(i => (Item: i, Rank: Rank(i, q)))
            .Where(x => x.Rank > 0)
            .OrderByDescending(x => x.Rank)
            .ThenBy(x => x.Item.FullName ?? x.Item.Uid, StringComparer.OrdinalIgnoreCase)
            .Take(Math.Max(1, limit))
            .Select(x => x.Item)
            .ToList();
    }

    /// <summary>
    ///     获取某个类型的直接成员（方法、属性、事件等）。
    /// </summary>
    /// <param name="typeUid">类型的 uid。</param>
    public IReadOnlyList<ApiItem> GetMembers(string typeUid)
    {
        ApiItem? type = GetByUid(typeUid);
        if (type?.Children is not { Count: > 0 } children)
            return [];

        Dictionary<string, ApiItem> byUid = EnsureLoaded();
        return children
            .Select(byUid.GetValueOrDefault)
            .Where(i => i is not null)
            .Select(i => i!)
            .ToList();
    }

    private static int Rank(ApiItem item, string query)
    {
        int rank = 0;
        if (string.Equals(item.Name, query, StringComparison.OrdinalIgnoreCase)) rank += 100;
        if (string.Equals(item.Uid, query, StringComparison.OrdinalIgnoreCase)) rank += 100;
        if (item.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) == true) rank += 30;
        if (item.FullName?.Contains(query, StringComparison.OrdinalIgnoreCase) == true) rank += 10;
        if (item.Uid.Contains(query, StringComparison.OrdinalIgnoreCase)) rank += 5;
        return rank;
    }

    private Dictionary<string, ApiItem> EnsureLoaded()
    {
        lock (_gate)
        {
            if (!Directory.Exists(_paths.ApiDirectory))
            {
                throw new DirectoryNotFoundException(
                    $"未找到 API 目录 {_paths.ApiDirectory}。请先运行 `docfx metadata` " +
                    $"（或 `docfx {_paths.Root}/docfx.json`）从源码 XML 文档注释生成 api/*.yml。");
            }

            string[] files = Directory.GetFiles(_paths.ApiDirectory, "*.yml", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                throw new FileNotFoundException(
                    $"API 目录 {_paths.ApiDirectory} 下没有 *.yml。请先运行 `docfx metadata` 生成。");
            }

            DateTime writeTime = files.Max(File.GetLastWriteTimeUtc);
            if (_byUid is not null && writeTime == _loadedWriteTimeUtc)
                return _byUid;

            Dictionary<string, ApiItem> byUid = new(StringComparer.Ordinal);
            foreach (string file in files)
            {
                ApiFile? parsed;
                try
                {
                    using StreamReader reader = new(file);
                    parsed = _deserializer.Deserialize<ApiFile>(reader);
                }
                catch
                {
                    // 单个文件解析失败不应影响整体；跳过。
                    continue;
                }

                if (parsed?.Items is null)
                    continue;

                foreach (ApiItem item in parsed.Items)
                {
                    if (!string.IsNullOrEmpty(item.Uid))
                        byUid[item.Uid] = item;
                }
            }

            _byUid = byUid;
            _loadedWriteTimeUtc = writeTime;
            return _byUid;
        }
    }
}
