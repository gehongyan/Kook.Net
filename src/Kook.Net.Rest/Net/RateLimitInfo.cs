using System.Globalization;

namespace Kook.Net;

/// <summary>
///     Represents a REST-Based ratelimit info.
/// </summary>
public struct RateLimitInfo : IRateLimitInfo
{
    /// <inheritdoc/>
    public bool IsGlobal { get; }

    /// <inheritdoc/>
    public int? Limit { get; }

    /// <inheritdoc/>
    public int? Remaining { get; }

    // /// <inheritdoc/>
    // public int? RetryAfter { get; }

    // /// <inheritdoc/>
    // public DateTimeOffset? Reset { get; }

    /// <inheritdoc/>
    public TimeSpan? ResetAfter { get; }

    /// <inheritdoc/>
    public string Bucket { get; }

    /// <inheritdoc/>
    public TimeSpan? Lag { get; }

    /// <inheritdoc/>
    public string Endpoint { get; }

    internal RateLimitInfo(Dictionary<string, string> headers, string endpoint)
    {
        Endpoint = endpoint;

        IsGlobal = headers.ContainsKey("X-Rate-Limit-Global");
        Limit = headers.TryGetValue("X-Rate-Limit-Limit", out string temp)
            && int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out int limit)
                ? limit
                : (int?)null;
        Remaining = headers.TryGetValue("X-Rate-Limit-Remaining", out temp)
            && int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out int remaining)
                ? remaining
                : (int?)null;
        // RetryAfter = headers.TryGetValue("Retry-After", out temp) &&
        //              int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var retryAfter) ? retryAfter : (int?)null;
        ResetAfter = headers.TryGetValue("X-Rate-Limit-Reset", out temp)
            && double.TryParse(temp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double resetAfter)
                ? TimeSpan.FromSeconds(resetAfter)
                : (TimeSpan?)null;
        // Reset = headers.TryGetValue("X-Rate-Limit-Reset", out temp) &&
        //         double.TryParse(temp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var reset) && reset != 0 ? DateTimeOffset.FromUnixTimeMilliseconds((long)(reset * 1000)) : (DateTimeOffset?)null;
        Bucket = headers.TryGetValue("X-Rate-Limit-Bucket", out temp) ? temp : null;
        Lag = headers.TryGetValue("Date", out temp)
            && DateTimeOffset.TryParse(temp, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset date)
                ? DateTimeOffset.UtcNow - date
                : (TimeSpan?)null;
    }
}
