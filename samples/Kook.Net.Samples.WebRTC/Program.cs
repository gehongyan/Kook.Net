using Kook;
using Kook.Commands;
using Kook.Net.Samples.WebRTC;
using Kook.Net.Samples.WebRTC.Services;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

// 这是一个使用 Kook.Net 实验性 WebRTC 语音功能的点歌机示例：
// 在语音频道的文字聊天中使用命令让机器人加入频道、点歌、切歌，
// 并且可以用命令录制频道里某个用户（或每一个人）的音频，每个用户保存为单独的 WAV 文件。所有事件都会输出详细日志到控制台。
//
// 命令（默认前缀为 !）：
//   help [命令]                         查看所有命令，或者某个命令的用法
//   join、leave                        加入、离开语音频道
//   add <链接>、addlist <歌单链接>、local <文件名>、skip、list    点歌机
//   record [@用户]、stoprecord [@用户]    开始、停止录制自己（不带参数）或指定的用户
//   recordall、stoprecordall           开始、停止录制每一个人，每个人的音频分别保存
//   recording                          查看谁正在被录制
// 录制别人、录制所有人需要「管理服务器」权限；任何人都可以停止对自己的录制。
//
// add 支持网易云音乐、QQ 音乐的歌曲页面链接，以及可以直接读取的音频链接；addlist 支持网易云音乐的歌单页面链接。
//
// 这是 Kook.Net.Experimental 提供的非官方文档化的功能，行为可能随时变化。
// 您需要安装 ffmpeg，Opus 编解码使用 Concentus，无需额外安装原生 Opus 库。
//
// 参数（命令行参数或环境变量）：
//   --TokenFile <path>          保存 Bot 令牌的文件。未指定时读取环境变量 KOOK_NET_TEST_TOKEN。
//   --Ffmpeg <path>             ffmpeg 可执行文件，默认 ffmpeg。
//   --MusicDirectory <path>     local 命令可以播放的本地音频所在的目录，未指定时 local 命令不可用。
//   --RecordingDirectory <dir>  每个用户的录音的保存目录，默认 ./recordings/<时间>。
//   --CommandPrefix <char>      命令前缀，默认 !。
//   --Quiet true                不输出 Kook.Net 音频调试器的日志（信令帧、ICE 与 DTLS 状态、收发的每个包等）。

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
WebRtcSampleOptions options = WebRtcSampleOptions.FromConfiguration(builder.Configuration);

// 日志的内容部分约定为 "[Tag] 消息 [Key] Value"，直接写在消息模板里，值同时是 Serilog 的结构化属性
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    // 启动失败的原因由下面的 catch 输出一行简洁的错误，不需要主机再输出一份带堆栈的
    .MinimumLevel.Override("Microsoft.Extensions.Hosting.Internal.Host", LogEventLevel.Fatal)
    .WriteTo.Console(LogEventLevel.Debug,
        "{Timestamp:HH:mm:ss.fff} {Level:u4} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

// Kook.Net 的音频调试器会以 [Tag] 消息 [Key] Value 的格式输出信令帧、ICE 与 DTLS 状态以及收发的每个音频包
if (!options.Quiet)
    KookDebugger.SetDebuggers(audio: true, debugger: (_, message) => Log.Debug("{Line}", message));

Log.Information("[Startup] Starting the WebRTC sample [Recording Directory] {RecordingDirectory} [Command Prefix] {CommandPrefix} [Music Directory] {MusicDirectory}",
    options.RecordingDirectory, options.CommandPrefix.ToString(), options.MusicDirectory);

builder.Services.AddSingleton(options);
builder.Services.AddSingleton(_ => new KookSocketClient(new KookSocketConfig
{
    LogLevel = LogSeverity.Debug,
    AlwaysDownloadUsers = true,
    AutoLogoutBeforeLogin = true
}));
builder.Services.AddSingleton(_ => new CommandService(new CommandServiceConfig
{
    DefaultRunMode = RunMode.Async
}));
// 托管服务按注册顺序启动，按相反顺序停止：停止时先让 MusicService 离开语音频道并结束录音，再登出
builder.Services.AddHostedService<KookClientService>();
builder.Services.AddHostedService<CommandHandlingService>();
builder.Services.AddSingleton<MusicLinkConverter>();
builder.Services.AddSingleton<MusicService>();
builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<MusicService>());

IHost app = builder.Build();
try
{
    await app.RunAsync();
}
catch (InvalidOperationException e)
{
    // 例如没有提供 Bot 令牌
    Log.Error("[Startup] {Error}", e.Message);
    return 1;
}

return 0;
