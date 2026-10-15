using Microsoft.Extensions.Configuration;

namespace Kook.Net.Samples.WebRTC;

/// <summary>
///     示例的配置，可以通过命令行参数（例如 <c>--TokenFile path</c>）或环境变量提供。
/// </summary>
public sealed class WebRtcSampleOptions
{
    /// <summary> 保存 Bot 令牌的文件。未指定时从环境变量 <see cref="TokenEnvironmentVariable"/> 读取。 </summary>
    public string? TokenFile { get; init; }

    /// <summary> 保存 Bot 令牌的环境变量的名称。 </summary>
    public string TokenEnvironmentVariable { get; init; } = "KOOK_NET_TEST_TOKEN";

    /// <summary> ffmpeg 可执行文件。 </summary>
    public string FfmpegPath { get; init; } = "ffmpeg";

    /// <summary> 命令前缀。 </summary>
    public char CommandPrefix { get; init; } = '!';

    /// <summary>
    ///     <c>local</c> 命令可以播放的本地音频所在的目录。未配置时 <c>local</c> 命令不可用。
    ///     聊天中的输入会交给 ffmpeg 读取，因此本地文件只能来自操作者明确指定的目录。
    /// </summary>
    public string? MusicDirectory { get; init; }

    /// <summary> 每个用户的录音（WAV 文件）的保存目录。 </summary>
    public required string RecordingDirectory { get; init; }

    /// <summary> 是否关闭 Kook.Net 音频调试器的输出（信令帧、ICE 与 DTLS 状态、收发的每个包等）。 </summary>
    public bool Quiet { get; init; }

    public static WebRtcSampleOptions FromConfiguration(IConfiguration configuration)
    {
        string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string? commandPrefix = configuration["CommandPrefix"];
        return new WebRtcSampleOptions
        {
            TokenFile = configuration["TokenFile"],
            TokenEnvironmentVariable = configuration["TokenEnvironmentVariable"] ?? "KOOK_NET_TEST_TOKEN",
            FfmpegPath = configuration["Ffmpeg"] ?? "ffmpeg",
            CommandPrefix = commandPrefix is { Length: 1 } ? commandPrefix[0] : '!',
            MusicDirectory = configuration["MusicDirectory"],
            RecordingDirectory = Path.GetFullPath(
                configuration["RecordingDirectory"] ?? Path.Combine("recordings", stamp)),
            Quiet = bool.TryParse(configuration["Quiet"], out bool quiet) && quiet
        };
    }

    public string ResolveBotToken()
    {
        if (!string.IsNullOrWhiteSpace(TokenFile))
        {
            string fromFile = File.ReadAllText(TokenFile).Trim();
            if (fromFile.Length == 0)
                throw new InvalidOperationException($"The token file is empty: {TokenFile}");
            return fromFile;
        }

        string? fromEnvironment = Environment.GetEnvironmentVariable(TokenEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(fromEnvironment))
            throw new InvalidOperationException(
                $"No bot token. Use --TokenFile or set the environment variable {TokenEnvironmentVariable}.");
        return fromEnvironment.Trim();
    }
}
