using System.Text.Json;
using System.Text.RegularExpressions;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     把网易云音乐和 QQ 音乐的歌曲、歌单页面链接转换为可以播放的音源。
/// </summary>
/// <remarks>
///     这些平台没有公开文档化可供第三方使用的获取音频的接口，这里使用的是它们网页端所用的接口，可能随时失效，
///     也可能违反平台的服务条款。VIP 歌曲、没有版权或所在地区受限的歌曲无法获取。仅作示例，请自行评估使用是否合适。
/// </remarks>
public sealed class MusicLinkConverter
{
    private const int MaxPlaylistTracks = 50;

    private static readonly Regex NeteaseSongPage = new(
        @"^https?://(y\.)?music\.163\.com/(#/|m/)?song\?id=(?<id>\d+)(&.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex NeteasePlaylistPage = new(
        @"^https?://(y\.)?music\.163\.com/(#/|m/)?playlist\?id=(?<id>\d+)(&.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex QqSongPage = new(
        @"^https?://y\.qq\.com/n/ryqq/songDetail/(?<id>[0-9A-Za-z]+)([/?#].*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    ///     尝试把歌曲页面链接转换为一首歌。
    /// </summary>
    /// <returns> 转换得到的歌；如果链接不是受支持的歌曲页面，则为 <c>null</c>。 </returns>
    /// <exception cref="InvalidOperationException"> 链接是受支持的歌曲页面，但歌曲无法播放。 </exception>
    public async Task<MusicTrack?> ConvertSongAsync(Uri url, CancellationToken cancellationToken = default)
    {
        Match match = NeteaseSongPage.Match(url.ToString());
        if (match.Success)
            return await ConvertNeteaseSongAsync(match.Groups["id"].Value, cancellationToken).ConfigureAwait(false);

        match = QqSongPage.Match(url.ToString());
        if (match.Success)
            return await ConvertQqSongAsync(match.Groups["id"].Value, cancellationToken).ConfigureAwait(false);

        return null;
    }

    /// <summary>
    ///     尝试把歌单页面链接转换为若干首歌。
    /// </summary>
    /// <returns> 转换得到的歌；如果链接不是受支持的歌单页面，则为 <c>null</c>。 </returns>
    /// <exception cref="InvalidOperationException"> 链接是受支持的歌单页面，但获取歌单失败。 </exception>
    public async Task<IReadOnlyList<MusicTrack>?> ConvertPlaylistAsync(Uri url, CancellationToken cancellationToken = default)
    {
        Match match = NeteasePlaylistPage.Match(url.ToString());
        if (!match.Success)
            return null;

        string id = match.Groups["id"].Value;
        using JsonDocument document = await GetJsonAsync($"https://music.163.com/api/playlist/detail?id={id}", cancellationToken)
            .ConfigureAwait(false);
        List<MusicTrack>? tracks;
        try
        {
            tracks = ParseNeteasePlaylist(document.RootElement);
        }
        catch (Exception e) when (e is KeyNotFoundException or InvalidOperationException or FormatException)
        {
            throw new InvalidOperationException("Unexpected response when fetching the playlist.", e);
        }

        return tracks ?? throw new InvalidOperationException("The playlist is not available.");
    }

    /// <returns> 歌单中的歌；如果服务器表示歌单不可用，则为 <c>null</c>。 </returns>
    private static List<MusicTrack>? ParseNeteasePlaylist(JsonElement root)
    {
        if (root.GetProperty("code").GetInt32() != 200)
            return null;

        List<MusicTrack> tracks = [];
        foreach (JsonElement track in root.GetProperty("result").GetProperty("tracks").EnumerateArray().Take(MaxPlaylistTracks))
            tracks.Add(new MusicTrack(FormatTitle(track), NeteaseOuterUrl(track.GetProperty("id").ToString())));
        return tracks;
    }

    private static string NeteaseOuterUrl(string id) => $"https://music.163.com/song/media/outer/url?id={id}.mp3";

    private async Task<MusicTrack> ConvertNeteaseSongAsync(string id, CancellationToken cancellationToken)
    {
        string source = NeteaseOuterUrl(id);
        // 外链会重定向到真实的音频地址，歌曲不可用时会重定向到一个错误页面，因此先确认一次
        await MusicSourceResolver.ResolvePlayableUriAsync(new Uri(source), cancellationToken).ConfigureAwait(false);
        return new MusicTrack(await GetNeteaseTitleAsync(id, cancellationToken).ConfigureAwait(false) ?? $"Netease song {id}", source);
    }

    private static async Task<string?> GetNeteaseTitleAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            using JsonDocument document = await GetJsonAsync($"https://music.163.com/api/song/detail/?id={id}&ids=%5B{id}%5D", cancellationToken)
                .ConfigureAwait(false);
            return FormatTitle(document.RootElement.GetProperty("songs")[0]);
        }
        catch (Exception e) when (e is InvalidOperationException or KeyNotFoundException or ArgumentOutOfRangeException or IndexOutOfRangeException)
        {
            // 标题只用于显示，获取失败不影响播放
            return null;
        }
    }

    private static string FormatTitle(JsonElement song)
    {
        string name = song.GetProperty("name").GetString() ?? "(unknown)";
        string[] artists = song.TryGetProperty("artists", out JsonElement artistsElement)
            ? artistsElement.EnumerateArray().Select(x => x.GetProperty("name").GetString() ?? string.Empty).ToArray()
            : [];
        return artists.Length == 0 ? name : $"{name} - {string.Join(" & ", artists)}";
    }

    private async Task<MusicTrack> ConvertQqSongAsync(string id, CancellationToken cancellationToken)
    {
        // 使用序列化生成请求的 JSON，而不是拼接字符串，避免歌曲 ID 注入内容
        string data = JsonSerializer.Serialize(new
        {
            req = new { module = "CDN.SrfCdnDispatchServer", method = "GetCdnDispatch", param = new { guid = "8348972662", calltype = 0, userip = "" } },
            req_0 = new
            {
                module = "vkey.GetVkeyServer", method = "CgiGetVkey",
                param = new { guid = "8348972662", songmid = new[] { id }, songtype = new[] { 1 }, uin = "0", loginflag = 1, platform = "20" }
            },
            comm = new { uin = 0, format = "json", ct = 24, cv = 0 }
        });
        string requestUrl = "https://u.y.qq.com/cgi-bin/musicu.fcg?g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8"
            + $"&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0&data={Uri.EscapeDataString(data)}";

        using JsonDocument document = await GetJsonAsync(requestUrl, cancellationToken).ConfigureAwait(false);
        string source;
        try
        {
            JsonElement root = document.RootElement;
            string[] hosts = root.GetProperty("req").GetProperty("data").GetProperty("sip").EnumerateArray()
                .Select(x => x.GetString() ?? string.Empty).Where(x => x.Length > 0).ToArray();
            JsonElement info = root.GetProperty("req_0").GetProperty("data").GetProperty("midurlinfo")[0];
            string path = info.GetProperty("purl").GetString() ?? string.Empty;
            // purl 为空表示匿名用户无法获取这首歌：需要 VIP、没有版权或需要登录，result 是服务器给出的原因代码
            if (hosts.Length == 0 || path.Length == 0)
            {
                string result = info.TryGetProperty("result", out JsonElement resultElement) ? resultElement.ToString() : "unknown";
                throw new InvalidOperationException(
                    $"The song is not available to anonymous users (code {result}). It may require VIP or login, or is not licensed.");
            }

            source = hosts[Random.Shared.Next(hosts.Length)] + path;
        }
        catch (Exception e) when (e is KeyNotFoundException or IndexOutOfRangeException or ArgumentOutOfRangeException)
        {
            throw new InvalidOperationException("Unexpected response when fetching the song.", e);
        }

        if (!Uri.TryCreate(source, UriKind.Absolute, out Uri? uri))
            throw new InvalidOperationException("Unexpected response when fetching the song.");
        // QQ 音乐的地址带有会过期的凭据，转换后立即确认并使用确认后的地址
        Uri playable = await MusicSourceResolver.ResolvePlayableUriAsync(uri, cancellationToken).ConfigureAwait(false);
        return new MusicTrack($"QQ Music song {id}", playable.AbsoluteUri);
    }

    private static async Task<JsonDocument> GetJsonAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await MusicSourceResolver.Http.GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"The server responded with {(int)response.StatusCode}.");
            await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException e)
        {
            throw new InvalidOperationException($"Failed to reach the server: {e.Message}", e);
        }
        catch (JsonException e)
        {
            throw new InvalidOperationException("The server returned an unexpected response.", e);
        }
        catch (TaskCanceledException e) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException("The server did not respond in time.", e);
        }
    }
}
