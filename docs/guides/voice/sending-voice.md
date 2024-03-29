---
uid: Guides.Voice.SendingVoice
title: 语音推流
---

## 前言

> [!WARNING]
> KOOK 尚未正式公开面向 Bot 的语音连接与推流的 API，当前的实现基于 [hank9999/kook-voice-API] 进行开发，在此向 [hank9999]
> 表示感谢。
>
> 使用语音连接与推流的功能，视为认同**开黑啦 V3 语音 API - 使用须知**：
>
> **注意本 API 由抓包得来，API 可能会随时变动进而失效**，
> **您需要得知，使用此 API 会违反 [KOOK 语音软件许可及服务协议] `3.2.3` 或 `3.2.5` 条款**，
> **同时会违反 [KOOK 开发者隐私政策] `数据信息` 或 `滥用` 中的相关条款**

[hank9999/kook-voice-API]: https://github.com/hank9999/kook-voice-API

[hank9999]: https://github.com/hank9999

[KOOK 语音软件许可及服务协议]:https://www.kookapp.cn/protocol.html

[KOOK 开发者隐私政策]: https://developer.kookapp.cn/doc/privacy

## 安装

语音推流需要 Opus 编码器的支持，要使用语音功能，请将 `opus` 原生库放在 Bot 运行目录内。

.NET Framework 中，请将该原生库放在编译或发布的输出目录中，例如 `bin/Debug`；.NET (Core) 中，该目录应为 `csporj` 文件所在目录。

Windows 开发者可以在[此处](https://github.com/gehongyan/Kook.Net/tree/master/voice-natives) 下载预编译的二进制文件。

Linux 开发者需要从源码编译 [Opus](http://downloads.xiph.org/releases/opus/)，或通过包管理器进行安装。

## 加入语音频道

语音推流前需要先加入语音频道，调用 [IAudioClient] 上的 [ConnectAsync]，该异步操作会返回一个 [IAudioClient] 对象，用于后续的语音推流操作。

[!code-csharp[加入语音频道](samples/joining_audio.cs)]

> [!WARNING]
> 改变语音状态的命令，例如加入或离开音频频道、推流时，应该使用 [RunMode.Async]，这可以防止在客户端的默认配置中产生死锁的反馈循环。
> 如果你能确保你的命令在与网关任务不同的任务中运行，那么也可以不需要 `RunMode.Async`。

加入语音频道后，客户端将保持与此频道的连接，直到被踢出频道、掉线、或其它被服务端通知需主动断开连接。

应注意的是，语音连接是基于每个服务器创建的。在单个服务器中，Bot 只能启动一个音频连接。
要在服务器内切换频道，需要在服务器的另一个语音频道上调用 [ConnectAsync]。

> [!WARNING]
> KOOK 已对 Bot 禁用对多个服务器的语音频道同时推流的能力，在某个服务器上保持音频连接时，如果在另一个服务器上的某个语音频道中调用
> [ConnectAsync]，或导致已有的音频链接被 KOOK 服务端中断。

[IAudioClient]: xref:Kook.Audio.IAudioClient
[ConnectAsync]: xref:Kook.IAudioChannel.ConnectAsync*
[RunMode.Async]: xref:Kook.Commands.RunMode

## 语音推流

### 通过 FFmpeg 转码

[FFmpeg] 是一个开源的、高度多功能的音视频混合工具。这是传输音频前进行转码的推荐方式。

在这之前，你需要安装 FFmpeg CLI，通常的做法是下载一个 FFmpeg 的版本，并将其放置在你的环境变量的 PATH 中（或者与 Bot 在同一位置，与
opus 在同一位置），参见 [FFmpeg 的下载页面]，或使用操作系统相应的包管理器。

[FFmpeg]: https://ffmpeg.org/

[FFmpeg 的下载页面]: https://ffmpeg.org/download.html

首先，创建一个启动 FFmpeg 的 Process 进程对象，来将输入音频以 PCM 方式转码为 48kHz 采样率的字节流。

[!code-csharp[启动 FFmpeg](samples/audio_create_ffmpeg.cs)]

该 ffmpeg 命令的参数中：

- `-hide_banner`：用于隐藏启动时的版权和版本信息。
- `-loglevel panic`：设置日志级别为 `panic`，只有最严重的错误才会被记录。
- `-i {source}`：指定输入文件或流，`{source}` 是输入的变量，表示具体的文件路径或网络地址。
- `-ac 2`：设置音频通道数量为 2，即立体声。
- `-f s16le`：设置输出格式为 16 位有符号小端（Signed 16-bit Little-Endian）PCM（脉冲编码调制）音频。
- `-ar 48000`：设置音频采样率为 48kHz。
- `pipe:1`：将输出重定向到标准输出，以便在接下来的操作中由程序读取。

### 由 Kook.Net 进行编码推流

接下来，要向 KOOK 传输音频，需要由 [IAudioClient] 创建一个 [AudioOutStream]，由于 ffmpeg 命令输出了 PCM
音频，因此使用 [IAudioClient.CreatePcmStream]。

[IAudioClient]: xref:Kook.Audio.IAudioClient
[AudioOutStream]: xref:Kook.Audio.AudioOutStream
[IAudioClient.CreatePCMStream]: xref:Kook.Audio.IAudioClient#Kook_Audio_IAudioClient_CreatePcmStream_Kook_Audio_AudioApplication_System_Int32_System_Int32_System_Int32_

最后，音频需要从 FFmpeg 的标准输出流传输到你的 AudioOutStream 对象中。
根据你的业务需要，这个步骤中间可能会进行某些处理，但在大多数情况下，使用 [Stream.CopyToAsync] 即可。

[Stream.CopyToAsync]: https://learn.microsoft.com/dotnet/api/system.io.stream.copytoasync

如果你正在实现一个点歌机，你可能会希望等待音频停止播放后再继续播放下一首歌，等待 `AudioOutStream.FlushAsync`
可以等待音频客户端的内部缓冲区清空。

[!code-csharp[音频推流](samples/audio_ffmpeg.cs)]
