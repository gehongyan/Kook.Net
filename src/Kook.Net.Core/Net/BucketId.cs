using System.Diagnostics.CodeAnalysis;
#if NET462
using System.Net.Http;
#endif

namespace Kook.Net;

/// <summary>
///     Represents a ratelimit bucket.
/// </summary>
public sealed class BucketId : IEquatable<BucketId>
{
    /// <summary>
    ///     Gets the http method used to make the request if available.
    /// </summary>
    public HttpMethod? HttpMethod { get; }

    /// <summary>
    ///     Gets the endpoint that is going to be requested if available.
    /// </summary>
    public string? Endpoint { get; }

    /// <summary>
    ///     Gets the major parameters of the route.
    /// </summary>
    public IOrderedEnumerable<KeyValuePair<string, string>> MajorParameters { get; }

    /// <summary>
    ///     Gets the hash of this bucket.
    /// </summary>
    /// <remarks>
    ///     The hash is provided by Kook to group ratelimits.
    /// </remarks>
    public string? BucketHash { get; }

    /// <summary>
    ///     Gets if this bucket is a hash type.
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
    ///     Creates a new <see cref="BucketId"/> based on the
    ///     <see cref="HttpMethod"/> and <see cref="Endpoint"/>.
    /// </summary>
    /// <param name="httpMethod">Http method used to make the request.</param>
    /// <param name="endpoint">Endpoint that is going to receive requests.</param>
    /// <param name="majorParams">Major parameters of the route of this endpoint.</param>
    /// <returns>
    ///     A <see cref="BucketId"/> based on the <see cref="HttpMethod"/>
    ///     and the <see cref="Endpoint"/> with the provided data.
    /// </returns>
    public static BucketId Create(HttpMethod httpMethod, string endpoint, Dictionary<string, string> majorParams)
    {
        Preconditions.NotNullOrWhitespace(endpoint, nameof(endpoint));
        majorParams ??= new Dictionary<string, string>();
        return new BucketId(httpMethod, endpoint, majorParams, null);
    }

    /// <summary>
    ///     Creates a new <see cref="BucketId"/> based on a
    ///     <see cref="BucketHash"/> and a previous <see cref="BucketId"/>.
    /// </summary>
    /// <param name="hash">Bucket hash provided by Kook.</param>
    /// <param name="oldBucket"><see cref="BucketId"/> that is going to be upgraded to a hash type.</param>
    /// <returns>
    ///     A <see cref="BucketId"/> based on the <see cref="BucketHash"/>
    ///     and <see cref="MajorParameters"/>.
    /// </returns>
    public static BucketId Create(string hash, BucketId oldBucket)
    {
        Preconditions.NotNullOrWhitespace(hash, nameof(hash));
        Preconditions.NotNull(oldBucket, nameof(oldBucket));
        return new BucketId(null, null, oldBucket.MajorParameters, hash);
    }

    /// <summary>
    ///     Gets the string that will define this bucket as a hash based one.
    /// </summary>
    /// <returns>
    ///     A string that defines this bucket as a hash based one.
    /// </returns>
    public string? GetBucketHash() => IsHashBucket
        ? $"{BucketHash}:{string.Join("/", MajorParameters.Select(x => x.Value))}"
        : null;

    /// <summary>
    ///     Gets the string that will define this bucket as an endpoint based one.
    /// </summary>
    /// <returns>
    ///     A string that defines this bucket as an endpoint based one.
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
