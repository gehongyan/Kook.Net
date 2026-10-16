using System.Net;
using System.Net.Sockets;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     把聊天中输入的内容解析为可以播放的歌，并在播放前确认网络音源是安全、可播放的。
/// </summary>
/// <remarks>
///     聊天中的输入是不可信的，而音源最终会交给 ffmpeg 读取，因此这里限制了可以读取的位置：
///     网络音源只允许 http(s) 链接，且拒绝解析到回环、内网、链路本地地址的主机；
///     重定向由这里手动逐跳跟随并逐跳检查，而不是交给 ffmpeg 自动跟随；
///     本地音源只能是 <see cref="WebRtcSampleOptions.MusicDirectory"/> 目录里的文件。
///     这不能防御 DNS 重绑定（检查之后域名解析到了别的地址），仅作示例，生产环境应当在网络层限制出站访问。
/// </remarks>
public static class MusicSourceResolver
{
    private const int MaxRedirects = 5;

    internal const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0 Safari/537.36";

    internal static readonly HttpClient Http = CreateHttpClient();

    private static HttpClient CreateHttpClient()
    {
        HttpClient client = new(new SocketsHttpHandler
        {
            AllowAutoRedirect = false,
            ConnectTimeout = TimeSpan.FromSeconds(10)
        })
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        return client;
    }

    /// <summary>
    ///     把用户输入的网络链接解析为一首歌。
    /// </summary>
    public static async Task<(MusicTrack? Track, string Error)> ResolveRemoteAsync(Uri uri,
        CancellationToken cancellationToken = default)
    {
        if (uri.Scheme is not ("http" or "https"))
            return (null, "Only http and https links are supported.");
        if (!await IsAllowedAsync(uri, cancellationToken).ConfigureAwait(false))
            return (null, "Links to local or private network addresses are not allowed.");
        return (new MusicTrack(uri.ToString(), uri.AbsoluteUri), string.Empty);
    }

    public static bool TryResolveLocal(string fileName, WebRtcSampleOptions options, out MusicTrack? track, out string error)
    {
        track = null;
        if (string.IsNullOrWhiteSpace(options.MusicDirectory))
        {
            error = "Local files are disabled. Start the sample with --MusicDirectory <path> to enable them.";
            return false;
        }

        string directory = Path.GetFullPath(options.MusicDirectory);
        string fullPath = Path.GetFullPath(Path.Combine(directory, fileName.Trim()));
        string prefix = directory.EndsWith(Path.DirectorySeparatorChar) ? directory : directory + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
        {
            error = "The file does not exist in the music directory.";
            return false;
        }

        track = new MusicTrack(Path.GetFileName(fullPath), fullPath);
        error = string.Empty;
        return true;
    }

    /// <summary>
    ///     手动跟随重定向，逐跳检查地址，确认最终得到的是一个可以读取的音频，并返回最终地址。
    /// </summary>
    /// <exception cref="InvalidOperationException"> 地址不允许访问、重定向过多、服务器返回错误，或者得到的不是音频。 </exception>
    public static async Task<Uri> ResolvePlayableUriAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        for (int hop = 0; hop <= MaxRedirects; hop++)
        {
            if (!await IsAllowedAsync(uri, cancellationToken).ConfigureAwait(false))
                throw new InvalidOperationException("Links to local or private network addresses are not allowed.");

            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, uri);
                // 只读取响应头，不下载内容
                using HttpResponseMessage response = await Http
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false);

                if ((int)response.StatusCode is >= 300 and < 400 && response.Headers.Location is { } location)
                {
                    uri = location.IsAbsoluteUri ? location : new Uri(uri, location);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException($"The server responded with {(int)response.StatusCode}.");

                // 歌曲不可用时，服务器常常返回一个 HTML 或 JSON 的错误页面
                string? mediaType = response.Content.Headers.ContentType?.MediaType;
                if (mediaType is not null
                    && (mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
                        || mediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("The link is not an audio file. The song may not be available.");

                return uri;
            }
            catch (HttpRequestException e)
            {
                throw new InvalidOperationException($"Failed to reach the server: {e.Message}", e);
            }
            catch (TaskCanceledException e) when (!cancellationToken.IsCancellationRequested)
            {
                throw new InvalidOperationException("The server did not respond in time.", e);
            }
        }

        throw new InvalidOperationException("Too many redirects.");
    }

    /// <summary>
    ///     确认链接是 http(s)，并且主机没有指向本机或内网。
    /// </summary>
    public static async Task<bool> IsAllowedAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        if (uri.Scheme is not ("http" or "https") || uri.IsLoopback
            || uri.Host.EndsWith(".local", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            return false;

        if (IPAddress.TryParse(uri.Host.Trim('[', ']'), out IPAddress? literal))
            return !IsInternalAddress(literal);

        try
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(uri.Host, cancellationToken).ConfigureAwait(false);
            return addresses.Length > 0 && addresses.All(x => !IsInternalAddress(x));
        }
        catch (SocketException)
        {
            return false;
        }
    }

    internal static bool IsInternalAddress(IPAddress address)
    {
        if (address.IsIPv4MappedToIPv6)
            address = address.MapToIPv4();
        if (IPAddress.IsLoopback(address) || address.IsIPv6LinkLocal || address.IsIPv6SiteLocal
            || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any))
            return true;
        if (address.AddressFamily == AddressFamily.InterNetworkV6)
        {
            // fc00::/7 唯一本地地址
            return (address.GetAddressBytes()[0] & 0xFE) == 0xFC;
        }

        byte[] bytes = address.GetAddressBytes();
        return bytes[0] switch
        {
            0 or 10 or 127 => true,
            169 => bytes[1] == 254,
            172 => bytes[1] is >= 16 and <= 31,
            192 => bytes[1] == 168,
            100 => bytes[1] is >= 64 and <= 127,
            _ => false
        };
    }
}
