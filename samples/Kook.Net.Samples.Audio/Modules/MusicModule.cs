using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Kook.Commands;
using Kook.Net.Samples.Audio.Services;
using Kook.WebSocket;

namespace Kook.Net.Samples.Audio.Modules;

/// <summary>
///     A module for music commands.
/// </summary>
public class MusicModule : ModuleBase<SocketCommandContext>
{
    private readonly MusicService _musicService;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MusicModule"/> class.
    /// </summary>
    public MusicModule(MusicService musicService, IHttpClientFactory httpClientFactory)
    {
        _musicService = musicService;
        _httpClientFactory = httpClientFactory;
    }

    [Command("join")]
    [RequireContext(ContextType.Guild)]
    public async Task JoinAsync()
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        await _musicService.ConnectAsync(voiceChannel);
        await ReplyTextAsync($"Connected to {voiceChannel.Name}.");
    }

    [Command("leave")]
    [RequireContext(ContextType.Guild)]
    public async Task LeaveAsync()
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        await _musicService.DisconnectAsync(voiceChannel);
        await ReplyTextAsync("Disconnected.");
    }

    // [Command("search")]
    // [RequireContext(ContextType.Guild)]
    // public async Task PlayAsync([Remainder] string query)
    // {
    //     if (string.IsNullOrWhiteSpace(query))
    //     {
    //         await ReplyTextAsync("Please provide search keywords or a link.");
    //         return;
    //     }
    //
    //     if (Context.Guild.AudioClient?.ConnectionState != ConnectionState.Connected)
    //     {
    //         await ReplyTextAsync("I'm not connected to a voice channel.");
    //         return;
    //     }
    //
    //     List<FullTrack> tracks = await _musicService.SearchAsync(query);
    //     if (tracks.Count == 0)
    //     {
    //         await ReplyTextAsync("No tracks found.");
    //         return;
    //     }
    //
    //     IEnumerable<string> results = tracks
    //         .Select((x, i) => $"[{i}] {x.Name} - {string.Join(" & ", x.Artists.Select(y => y.Name))}");
    //     string result = string.Join(Environment.NewLine, new List<string> { $"Querying: {query}" }.Concat(results));
    //     await ReplyTextAsync(result);
    // }

    // [Command("add")]
    // public async Task AddAsync(int index)
    // {
    //     if (Context.Guild.AudioClient?.ConnectionState != ConnectionState.Connected)
    //     {
    //         await ReplyTextAsync("I'm not connected to a voice channel.");
    //         return;
    //     }
    //
    //     IMessage queryResult = await Context.Channel.GetMessageAsync(Context.Message.Quote.QuotedMessageId);
    //     if (Context.Message.Quote.Author.Id != Context.Client.CurrentUser.Id
    //         || !queryResult.Content.StartsWith("Querying: "))
    //     {
    //         await ReplyTextAsync("You must use the search command first.");
    //         return;
    //     }
    //
    //     MatchCollection matches = Regex.Matches(queryResult.Content, @"^\[(?<id>\d+)] (?<item>.*)$", RegexOptions.Multiline);
    //     if (matches.Count == 0 || index < 0 || index >= matches.Count)
    //     {
    //         await ReplyTextAsync("Invalid index.");
    //         return;
    //     }
    //
    //     string item = matches[index].Groups["item"].Value;
    // }

    [Command("add")]
    [RequireContext(ContextType.Guild)]
    public async Task AddAsync([Remainder] Uri url)
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        Uri? parsed = await ConvertSongUriAsync(url.ToString());
        if (parsed is null)
        {
            await ReplyTextAsync("Failed to convert the URL to a direct link.");
            return;
        }

        _musicService.Enqueue(voiceChannel, parsed);
        await ReplyTextAsync($"Added: {parsed}");
    }

    [Command("addlist")]
    [RequireContext(ContextType.Guild)]
    public async Task AddListAsync([Remainder] Uri url)
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        List<Uri>? parsed = await ConvertSongListUriAsync(url.ToString());
        if (parsed is null)
        {
            await ReplyTextAsync("Failed to passe the URL to a link representing a song list.");
            return;
        }

        _musicService.Enqueue(voiceChannel, parsed);
        await ReplyTextAsync($"Added: {parsed.Count} songs.");
    }

    [Command("addraw")]
    [Alias("raw")]
    [RequireContext(ContextType.Guild)]
    public async Task AddRawAsync([Remainder] Uri url)
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        _musicService.Enqueue(voiceChannel, url);
        await ReplyTextAsync($"Added: {url}");
    }

    [Command("skip")]
    [RequireContext(ContextType.Guild)]
    public async Task SkipAsync()
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        _musicService.Skip(voiceChannel);
        await ReplyTextAsync("Skipped.");
    }

    [Command("list")]
    [RequireContext(ContextType.Guild)]
    public async Task ListAsync()
    {
        if (Context.Message.Source is not MessageSource.User) return;
        if (Context.Channel is not SocketVoiceChannel voiceChannel)
        {
            await ReplyTextAsync("You muse use this command in a voice channel.");
            return;
        }

        IEnumerable<Uri> queue = _musicService.GetQueue(voiceChannel);
        await ReplyTextAsync(string.Join(Environment.NewLine, queue.Select((x, i) => $"[{i}] {x}")));
    }

    private static readonly Regex neteaseSongPageRegex = new(@"^https://music.163.com(/#)?/song\?id=(?<id>\d+)(&.*)?$", RegexOptions.Compiled);
    private static readonly Regex qqSongPageRegex = new(@"^https://y.qq.com/n/ryqq/songDetail/(?<id>\w+)(&.*)?$", RegexOptions.Compiled);

    private async Task<Uri?> ConvertSongUriAsync(string url)
    {
        // 网易云音乐歌曲页面
        Match match = neteaseSongPageRegex.Match(url);
        if (match.Success)
        {
            string id = match.Groups["id"].Value;
            return new Uri($"https://music.163.com/song/media/outer/url?id={id}.mp3");
        }

        // QQ 音乐歌曲页面
        match = qqSongPageRegex.Match(url);
        if (match.Success)
        {
            string id = match.Groups["id"].Value;
            HttpClient httpClient = _httpClientFactory.CreateClient("Music");
            JsonDocument? response = await httpClient.GetFromJsonAsync<JsonDocument>(
                $$$"""https://u.y.qq.com/cgi-bin/musicu.fcg?g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0&data={"req":{"module":"CDN.SrfCdnDispatchServer","method":"GetCdnDispatch","param":{"guid":"8348972662","calltype":0,"userip":""}},"req_0":{"module":"vkey.GetVkeyServer","method":"CgiGetVkey","param":{"guid":"8348972662","songmid":["{{{id}}}"],"songtype":[1],"uin":"0","loginflag":1,"platform":"20"}},"comm":{"uin":0,"format":"json","ct":24,"cv":0}}""");
            if (response is null) return null;
            List<string> sips = response.RootElement
                .GetProperty("req").GetProperty("data").GetProperty("sip")
                .EnumerateArray()
                .Select(x => x.ToString())
                .ToList();
            string sip = sips[Random.Shared.Next(sips.Count)];
            string path = response.RootElement
                .GetProperty("req_0").GetProperty("data").GetProperty("midurlinfo")[0].GetProperty("purl")
                .ToString();
            return new Uri($"{sip}{path}");
        }

        return null;
    }

    private static readonly Regex neteaseSongListPageRegex = new(@"^https://music.163.com(/#)?/playlist\?id=(?<id>\d+)(&.*)?$", RegexOptions.Compiled);

    private async Task<List<Uri>?> ConvertSongListUriAsync(string url)
    {
        // 网易云歌单页面
        Match match = neteaseSongListPageRegex.Match(url);
        if (match.Success)
        {
            string id = match.Groups["id"].Value;
            HttpClient httpClient = _httpClientFactory.CreateClient("Music");
            JsonDocument? response = await httpClient.GetFromJsonAsync<JsonDocument>(
                $"https://music.163.com/api/playlist/detail?id={id}");
            if (response is null) return null;
            if (response.RootElement.GetProperty("code").GetInt32() != 200)
                throw new InvalidOperationException(response.RootElement.GetProperty("message").ToString());
            List<Uri> songs = response.RootElement
                .GetProperty("result").GetProperty("tracks").EnumerateArray()
                .Select(x => x.GetProperty("id").ToString())
                .Select(x => $"https://music.163.com/song/media/outer/url?id={x}.mp3")
                .Select(x => new Uri(x))
                .ToList();
            return songs;
        }

        return null;
    }
}
