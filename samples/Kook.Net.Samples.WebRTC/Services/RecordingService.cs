using System.Collections.Concurrent;
using Kook.Audio;
using Microsoft.Extensions.Logging;

namespace Kook.Net.Samples.WebRTC.Services;

/// <summary>
///     管理一个语音频道里收到的音频流的录制：谁被录制由命令决定，每个用户一个文件，每个音频流一个读取任务。
/// </summary>
/// <remarks>
///     成员离开频道再加入时，服务器会为他创建新的音频流、分配新的 SSRC，因此录制的目标是「用户」，而不是某个音频流：
///     只要用户仍然是录制目标，他之后新建的音频流会继续写入同一个文件。
/// </remarks>
public sealed class RecordingService
{
    private sealed record Reader(ulong UserId, UserRecorder Recorder, CancellationTokenSource Cancellation, Task Task);

    private readonly string _directory;
    private readonly ILogger<RecordingService> _logger;
    private readonly Lock _gate = new();
    private readonly HashSet<ulong> _targets = [];
    private readonly HashSet<ulong> _excluded = [];
    private readonly Dictionary<ulong, UserRecorder> _recorders = [];
    private readonly ConcurrentDictionary<uint, Reader> _readers = new();
    private bool _recordAll;

    public RecordingService(string directory, ILogger<RecordingService> logger)
    {
        _directory = directory;
        _logger = logger;
    }

    /// <summary> 是否在录制每一个人。 </summary>
    public bool RecordAll
    {
        get
        {
            lock (_gate)
                return _recordAll;
        }
    }

    /// <summary> 被单独指定为录制目标的用户。 </summary>
    public IReadOnlyCollection<ulong> Targets
    {
        get
        {
            lock (_gate)
                return [.. _targets];
        }
    }

    /// <summary> 用户当前是否是录制目标。 </summary>
    public bool ShouldRecord(ulong userId)
    {
        lock (_gate)
            return _targets.Contains(userId) || (_recordAll && !_excluded.Contains(userId));
    }

    /// <summary> 开始录制一个用户。 </summary>
    /// <returns> 如果用户已经是录制目标，则为 <c>false</c>。 </returns>
    public bool StartUser(ulong userId)
    {
        lock (_gate)
        {
            bool alreadyRecording = ShouldRecordLocked(userId);
            _excluded.Remove(userId);
            _targets.Add(userId);
            return !alreadyRecording;
        }
    }

    /// <summary> 开始录制频道里的每一个人，每个人的音频分别保存到各自的文件。 </summary>
    /// <returns> 如果已经在录制每一个人，则为 <c>false</c>。 </returns>
    public bool StartAll()
    {
        lock (_gate)
        {
            if (_recordAll)
                return false;
            _recordAll = true;
            _excluded.Clear();
            return true;
        }
    }

    /// <summary> 停止录制一个用户，并保存他的文件。 </summary>
    /// <returns> 如果用户本来就不在录制，则为 <c>false</c>。 </returns>
    public async Task<bool> StopUserAsync(ulong userId)
    {
        UserRecorder? recorder;
        lock (_gate)
        {
            bool wasRecording = ShouldRecordLocked(userId);
            _targets.Remove(userId);
            // 正在录制每一个人时，单独停止某个人需要把他排除在外，否则他下一次开麦又会被录制
            if (_recordAll)
                _excluded.Add(userId);
            if (!wasRecording)
                return false;
            _recorders.Remove(userId, out recorder);
        }

        foreach ((uint ssrc, Reader reader) in _readers.ToArray())
        {
            if (reader.UserId == userId)
                await StopReaderAsync(ssrc).ConfigureAwait(false);
        }

        if (recorder is not null)
            Save(recorder);
        return true;
    }

    /// <summary> 停止所有录制，并保存所有文件。 </summary>
    public async Task CompleteAsync()
    {
        UserRecorder[] recorders;
        lock (_gate)
        {
            _recordAll = false;
            _targets.Clear();
            _excluded.Clear();
            recorders = [.. _recorders.Values];
            _recorders.Clear();
        }

        foreach (uint ssrc in _readers.Keys.ToArray())
            await StopReaderAsync(ssrc).ConfigureAwait(false);

        foreach (UserRecorder recorder in recorders)
            Save(recorder);
        _logger.LogInformation("[Recording] Stopped all recordings [Saved Files] {SavedFiles}", recorders.Length);
    }

    /// <summary>
    ///     如果用户是录制目标，开始读取他的音频流并录制。
    /// </summary>
    /// <param name="userId"> 音频流所属的用户的 ID。 </param>
    /// <param name="displayName"> 用户的显示名称，用于文件名。 </param>
    /// <param name="ssrc"> 音频流的 SSRC。 </param>
    /// <param name="stream"> 音频输入流。 </param>
    public void TryAttach(ulong userId, string? displayName, uint ssrc, AudioInStream stream)
    {
        UserRecorder recorder;
        lock (_gate)
        {
            if (!ShouldRecordLocked(userId) || _readers.ContainsKey(ssrc))
                return;

            if (!_recorders.TryGetValue(userId, out UserRecorder? existing))
            {
                string fileName = $"user-{userId}-{SanitizeFileName(displayName ?? "unknown")}-{DateTime.Now:yyyyMMdd-HHmmss-fff}.wav";
                string path = Path.Combine(_directory, fileName);
                Directory.CreateDirectory(_directory);
                existing = new UserRecorder(userId, displayName, path);
                _recorders[userId] = existing;
                _logger.LogInformation("[Recording] Started recording a user [User ID] {UserId} [Display Name] {DisplayName} [File] {File}",
                    userId, displayName, path);
            }
            recorder = existing;

            recorder.BeginSegment();
            CancellationTokenSource cancellation = new();
            Task task = Task.Run(() => ReadAsync(recorder, ssrc, stream, cancellation.Token));
            _readers[ssrc] = new Reader(userId, recorder, cancellation, task);
        }

        _logger.LogInformation("[Recording] Started reading an audio stream [User ID] {UserId} [SSRC] {Ssrc} [Segment] {Segment}",
            userId, ssrc, recorder.Segments);
    }

    /// <summary>
    ///     停止读取一个音频流。用户仍是录制目标时，他的文件会保持打开，等他的下一个音频流继续写入。
    /// </summary>
    public Task StopStreamAsync(uint ssrc) => StopReaderAsync(ssrc);

    private bool ShouldRecordLocked(ulong userId) =>
        _targets.Contains(userId) || (_recordAll && !_excluded.Contains(userId));

    private async Task StopReaderAsync(uint ssrc)
    {
        if (!_readers.TryRemove(ssrc, out Reader? reader))
            return;
        await reader.Cancellation.CancelAsync().ConfigureAwait(false);
        await reader.Task.ConfigureAwait(false);
        reader.Cancellation.Dispose();
        _logger.LogInformation("[Recording] Stopped reading an audio stream [User ID] {UserId} [SSRC] {Ssrc} [Frames] {Frames} [Duration Seconds] {DurationSeconds}",
            reader.UserId, ssrc, reader.Recorder.Frames, Math.Round(reader.Recorder.DurationSeconds, 2));
    }

    private void Save(UserRecorder recorder)
    {
        recorder.Dispose();
        _logger.LogInformation(
            "[Recording] Saved the audio of a user [User ID] {UserId} [Display Name] {DisplayName} [File] {File} [Segments] {Segments} [Frames] {Frames} [Dropped Frames] {DroppedFrames} [Duration Seconds] {DurationSeconds} [Inserted Silence Seconds] {InsertedSilenceSeconds}",
            recorder.UserId, recorder.DisplayName, recorder.Path, recorder.Segments, recorder.Frames, recorder.DroppedFrames,
            Math.Round(recorder.DurationSeconds, 2), Math.Round(recorder.SilenceSeconds, 2));
    }

    private async Task ReadAsync(UserRecorder recorder, uint ssrc, AudioInStream stream, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                RtpFrame frame = await stream.ReadFrameAsync(cancellationToken).ConfigureAwait(false);
                recorder.WriteFrame(frame);
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        catch (ObjectDisposedException)
        {
            // ignored
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Recording] Failed to read an audio stream [SSRC] {Ssrc}", ssrc);
        }
    }

    private static string SanitizeFileName(string name)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        string cleaned = new(name.Select(c => invalid.Contains(c) || char.IsWhiteSpace(c) ? '_' : c).ToArray());
        return cleaned.Length > 40 ? cleaned.Substring(0, 40) : cleaned;
    }
}
