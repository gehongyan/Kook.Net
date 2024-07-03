using System.Diagnostics.CodeAnalysis;
#if NET462
using System.Net.Http;
#endif

namespace Kook.Net;

/// <summary>
///     表示一个限速桶。
/// </summary>
public sealed class BucketId : IEquatable<BucketId>
{
    /// <summary>
    ///     获取用于发起请求的 HTTP 方法（如果可用）。
    /// </summary>
    public HttpMethod? HttpMethod { get; }

    /// <summary>
    ///     获取将要请求的终结点（如果可用）。
    /// </summary>
    public string? Endpoint { get; }

    /// <summary>
    ///     获取路由的主要参数。
    /// </summary>
    public IOrderedEnumerable<KeyValuePair<string, string>> MajorParameters { get; }

    /// <summary>
    ///     获取此桶的哈希值。
    /// </summary>
    /// <remarks>
    ///     此哈希值由 KOOK 提供，用于分组限速。
    /// </remarks>
    public string? BucketHash { get; }

    /// <summary>
    ///     获取此限速桶是否为哈希分组限速桶。
    /// </summary>
    public bool IsHashBucket => BucketHash != null;

    private BucketId(HttpMethod? httpMethod, string? endpoint, IEnumerable<KeyValuePair<string, string>> majorParameters, string? bucketHash)
    {
        HttpMethod = httpMethod;
        Endpoint = endpoint;
        MajorParameters = majorParameters.OrderBy(x => x.Key);
        BucketHash = bucketHash;
    }

    /// <summary>
    ///     基于 <see cref="HttpMethod"/> 和 <see cref="Endpoint"/> 创建一个新的 <see cref="BucketId"/>。
    /// </summary>
    /// <param name="httpMethod">  用于发送请求的 HTTP 方法。 </param>
    /// <param name="endpoint"> 请求的终结点。 </param>
    /// <param name="majorParams"> 终结点的主要路由参数。 </param>
    /// <returns>
    ///     一个基于指定的 <see cref="HttpMethod"/> 和 <see cref="Endpoint"/> 创建的 <see cref="BucketId"/>。
    /// </returns>
    public static BucketId Create(HttpMethod? httpMethod, string? endpoint, Dictionary<string, string>? majorParams)
    {
        Preconditions.NotNullOrWhitespace(endpoint, nameof(endpoint));
        majorParams ??= new Dictionary<string, string>();
        return new BucketId(httpMethod, endpoint, majorParams, null);
    }

    /// <summary>
    ///     基于 <see cref="BucketHash"/> 和之前的 <see cref="BucketId"/> 创建一个新的 <see cref="BucketId"/>。
    /// </summary>
    /// <param name="hash"> 由 KOOK 提供的分组限速哈希值。 </param>
    /// <param name="oldBucket"> 要被升级为哈希分组限速桶的已有限速桶。 </param>
    /// <returns>
    ///     一个基于指定的 <see cref="BucketHash"/> 和之前的 <see cref="BucketId"/> 创建的 <see cref="BucketId"/>。
    /// </returns>
    public static BucketId Create(string hash, BucketId oldBucket)
    {
        Preconditions.NotNullOrWhitespace(hash, nameof(hash));
        Preconditions.NotNull(oldBucket, nameof(oldBucket));
        return new BucketId(null, null, oldBucket.MajorParameters, hash);
    }

    /// <summary>
    ///     获取将此桶定义为哈希分组限速桶的字符串。
    /// </summary>
    /// <returns>
    ///     A string that defines this bucket as a hash based one.
    ///     如果此桶是哈希分组限速桶，则返回此桶的哈希值；否则返回 <see langword="null"/>。
    /// </returns>
    public string? GetBucketHash() => IsHashBucket
        ? $"{BucketHash}:{string.Join("/", MajorParameters.Select(x => x.Value))}"
        : null;

    /// <summary>
    ///     获取将此桶定义为终结点限速桶的字符串。
    /// </summary>
    /// <returns>
    ///     如果此桶是终结点限速桶，则返回此桶的终结点；否则返回 <see langword="null"/>。
    /// </returns>
    public string? GetUniqueEndpoint() => HttpMethod != null ? $"{HttpMethod} {Endpoint}" : Endpoint;

    /// <inheritdoc />
    public override int GetHashCode() => IsHashBucket
        ? (BucketHash, string.Join("/", MajorParameters.Select(x => x.Value))).GetHashCode()
        : (HttpMethod, Endpoint).GetHashCode();

    /// <inheritdoc />
    public override string? ToString() => GetBucketHash() ?? GetUniqueEndpoint();

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not BucketId bucketId)
            return false;
        return Equals(bucketId);
    }

    /// <inheritdoc />
    public bool Equals(BucketId? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        if (GetType() != other.GetType())
            return false;
        return ToString() == other.ToString();
    }
}
