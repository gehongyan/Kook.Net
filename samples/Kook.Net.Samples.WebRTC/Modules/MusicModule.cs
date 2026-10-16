using System.Diagnostics.CodeAnalysis;
using Kook.Commands;
using Kook.Net.Samples.WebRTC.Services;
using Kook.WebSocket;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.WebRTC.Modules;

/// <summary>
///     点歌机的命令。需要在机器人要加入的语音频道的文字聊天中使用。
/// </summary>
public class MusicModule : ModuleBase<SocketCommandContext>
{
    private readonly MusicService _musicService;
    private readonly MusicLinkConverter _linkConverter;
    private readonly WebRtcSampleOptions _options;
    private readonly ILogger<MusicModule> _logger;

    public MusicModule(MusicService musicService, MusicLinkConverter linkConverter, WebRtcSampleOptions options,
        ILogger<MusicModule> logger)
    {
        _musicService = musicService;
        _linkConverter = linkConverter;
        _options = options;
        _logger = logger;
    }

    [Command("join")]
    [Summary("Join this voice channel. Use it in the text chat of the voice channel.")]
    [RequireContext(ContextType.Guild)]
    public async Task JoinAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await RunAsync(async () =>
        {
            await _musicService.ConnectAsync(voiceChannel);
            await ReplyTextAsync($"Connected to {voiceChannel.Name}. Nobody is being recorded. Use the record commands to start.");
        });
    }

    [Command("leave")]
    [Summary("Leave the voice channel and save all recordings.")]
    [RequireContext(ContextType.Guild)]
    public async Task LeaveAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await _musicService.DisconnectAsync(voiceChannel.Id);
        await ReplyTextAsync("Disconnected.");
    }

    [Command("add")]
    [Summary("Add a song to the queue. Supports NetEase Cloud Music and QQ Music song pages, and direct audio links.")]
    [RequireContext(ContextType.Guild)]
    public async Task AddAsync([Remainder] Uri url)
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await RunAsync(async () =>
        {
            // 先尝试网易云音乐、QQ 音乐的歌曲页面，不是的话当作可以直接读取的音频链接
            MusicTrack? track = await _linkConverter.ConvertSongAsync(url);
            if (track is null)
            {
                (track, string error) = await MusicSourceResolver.ResolveRemoteAsync(url);
                if (track is null)
                {
                    await ReplyTextAsync(error);
                    return;
                }
            }

            _musicService.Enqueue(voiceChannel, track);
            await ReplyTextAsync($"Added: {track.Title}");
        });
    }

    [Command("addlist")]
    [Summary("Add a NetEase Cloud Music playlist (up to 50 songs) to the queue.")]
    [RequireContext(ContextType.Guild)]
    public async Task AddListAsync([Remainder] Uri url)
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await RunAsync(async () =>
        {
            IReadOnlyList<MusicTrack>? tracks = await _linkConverter.ConvertPlaylistAsync(url);
            if (tracks is null)
            {
                await ReplyTextAsync("Only NetEase Cloud Music playlist links are supported.");
                return;
            }

            foreach (MusicTrack track in tracks)
                _musicService.Enqueue(voiceChannel, track);
            await ReplyTextAsync($"Added: {tracks.Count} songs.");
        });
    }

    [Command("local")]
    [Summary("Add an audio file from the configured music directory to the queue.")]
    [RequireContext(ContextType.Guild)]
    public async Task AddLocalAsync([Remainder] string fileName)
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        if (!MusicSourceResolver.TryResolveLocal(fileName, _options, out MusicTrack? track, out string error))
        {
            await ReplyTextAsync(error);
            return;
        }

        await RunAsync(async () =>
        {
            _musicService.Enqueue(voiceChannel, track!);
            await ReplyTextAsync($"Added: {track!.Title}");
        });
    }

    /// <summary> 开始录制自己，或者指定的用户。录制别人需要「管理服务器」权限。 </summary>
    [Command("record")]
    [Summary("Start recording yourself, or the mentioned user (needs the Manage Guild permission). Each user is saved to a separate file.")]
    [RequireContext(ContextType.Guild)]
    public async Task RecordAsync(SocketGuildUser? user = null)
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }
        if (Context.User is not SocketGuildUser sender) return;

        SocketGuildUser target = user ?? sender;
        if (target.Id != sender.Id && !CanManageRecording(sender))
        {
            await ReplyTextAsync("You need the Manage Guild permission to record other users.");
            return;
        }

        await RunAsync(async () =>
        {
            bool started = _musicService.StartRecording(voiceChannel, target.Id);
            await ReplyTextAsync(started
                ? $"Recording the audio of {target.DisplayName} to a local file. {target.DisplayName} can stop it at any time with the stoprecord command."
                : $"Already recording the audio of {target.DisplayName}.");
        });
    }

    /// <summary> 停止录制自己，或者指定的用户，并保存文件。任何人都可以停止对自己的录制。 </summary>
    [Command("stoprecord")]
    [Summary("Stop recording yourself, or the mentioned user (needs the Manage Guild permission), and save the file.")]
    [RequireContext(ContextType.Guild)]
    public async Task StopRecordAsync(SocketGuildUser? user = null)
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }
        if (Context.User is not SocketGuildUser sender) return;

        SocketGuildUser target = user ?? sender;
        if (target.Id != sender.Id && !CanManageRecording(sender))
        {
            await ReplyTextAsync("You need the Manage Guild permission to stop the recording of other users.");
            return;
        }

        await RunAsync(async () =>
        {
            bool stopped = await _musicService.StopRecordingAsync(voiceChannel, target.Id);
            await ReplyTextAsync(stopped
                ? $"Stopped recording {target.DisplayName} and saved the file."
                : $"{target.DisplayName} is not being recorded.");
        });
    }

    /// <summary> 开始录制频道里的每一个人，每个人的音频分别保存为单独的文件。需要「管理服务器」权限。 </summary>
    [Command("recordall")]
    [Summary("Start recording everyone in the channel, each user to a separate file (needs the Manage Guild permission).")]
    [RequireContext(ContextType.Guild)]
    public async Task RecordAllAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }
        if (Context.User is not SocketGuildUser sender) return;

        if (!CanManageRecording(sender))
        {
            await ReplyTextAsync("You need the Manage Guild permission to record everyone.");
            return;
        }

        await RunAsync(async () =>
        {
            bool started = _musicService.StartRecordingAll(voiceChannel);
            await ReplyTextAsync(started
                ? "Recording everyone in this channel, each user to a separate local file. Anyone can stop the recording of themselves with the stoprecord command."
                : "Already recording everyone.");
        });
    }

    /// <summary> 停止所有录制并保存所有文件。需要「管理服务器」权限。 </summary>
    [Command("stoprecordall")]
    [Summary("Stop all recordings and save the files (needs the Manage Guild permission).")]
    [RequireContext(ContextType.Guild)]
    public async Task StopRecordAllAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }
        if (Context.User is not SocketGuildUser sender) return;

        if (!CanManageRecording(sender))
        {
            await ReplyTextAsync("You need the Manage Guild permission to stop all recordings.");
            return;
        }

        await RunAsync(async () =>
        {
            await _musicService.StopRecordingAllAsync(voiceChannel);
            await ReplyTextAsync("Stopped all recordings and saved the files.");
        });
    }

    /// <summary> 查看谁正在被录制。 </summary>
    [Command("recording")]
    [Summary("Show who is being recorded.")]
    [RequireContext(ContextType.Guild)]
    public async Task RecordingStatusAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await RunAsync(async () =>
        {
            (bool all, IReadOnlyCollection<ulong> targets) = _musicService.GetRecordingStatus(voiceChannel);
            string names = string.Join(", ", targets.Select(x => voiceChannel.Guild.GetUser(x)?.DisplayName ?? x.ToString()));
            await ReplyTextAsync(all ? "Recording everyone."
                : targets.Count > 0 ? $"Recording: {names}"
                : "Nobody is being recorded.");
        });
    }

    private static bool CanManageRecording(SocketGuildUser user) =>
        user.GuildPermissions.Administrator || user.GuildPermissions.ManageGuild;

    [Command("skip")]
    [Summary("Skip the song that is playing.")]
    [RequireContext(ContextType.Guild)]
    public async Task SkipAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await RunAsync(async () =>
            await ReplyTextAsync(_musicService.Skip(voiceChannel) ? "Skipped." : "Nothing is playing."));
    }

    [Command("list")]
    [Summary("Show the song that is playing and the queue.")]
    [RequireContext(ContextType.Guild)]
    public async Task ListAsync()
    {
        if (!TryGetVoiceChannel(out SocketVoiceChannel? voiceChannel)) { await ReplyNotVoiceChannelAsync(); return; }

        await RunAsync(async () =>
        {
            (MusicTrack? current, IReadOnlyList<MusicTrack> queue) = _musicService.GetQueue(voiceChannel);
            List<string> lines = [$"Now playing: {current?.Title ?? "(nothing)"}"];
            lines.AddRange(queue.Select((x, i) => $"[{i}] {x.Title}"));
            await ReplyTextAsync(string.Join(Environment.NewLine, lines));
        });
    }

    private bool TryGetVoiceChannel([NotNullWhen(true)] out SocketVoiceChannel? voiceChannel)
    {
        // 机器人只响应真实用户的命令
        if (Context.Message.Source is MessageSource.User && Context.Channel is SocketVoiceChannel channel)
        {
            voiceChannel = channel;
            return true;
        }

        voiceChannel = null;
        return false;
    }

    private Task ReplyNotVoiceChannelAsync() =>
        Context.Message.Source is MessageSource.User
            ? ReplyTextAsync("You must use this command in a voice channel.")
            : Task.CompletedTask;

    private async Task RunAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (InvalidOperationException e)
        {
            await ReplyTextAsync(e.Message);
        }
        catch (TimeoutException e)
        {
            _logger.LogError(e, "[Command] Failed to connect to the voice channel");
            await ReplyTextAsync("Failed to connect to the voice channel. Check the permissions of the bot.");
        }
        catch (HttpException e)
        {
            // 例如机器人没有连接语音频道的权限时，服务器会返回 403
            _logger.LogError(e, "[Command] The server rejected the request");
            await ReplyTextAsync($"The server rejected the request: {e.Message}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Command] A command threw an exception");
            await ReplyTextAsync("Something went wrong. Check the log of the sample.");
        }
    }
}
