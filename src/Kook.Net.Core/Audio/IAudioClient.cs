namespace Kook.Audio;

/// <summary>
///     Represents a generic audio client.
/// </summary>
public interface IAudioClient : IDisposable
{
    /// <summary>
    ///     Occurs when the client has connected to the voice server.
    /// </summary>
    event Func<Task> Connected;

    /// <summary>
    ///     Occurs when the client has disconnected from the voice server.
    ///     The first <see cref="Exception"/> parameter is the exception that caused the disconnection.
    /// </summary>
    event Func<Exception, Task> Disconnected;

    // /// <summary>
    // ///     Occurs when the latency of the voice WebSocket server has been updated.
    // /// </summary>
    // event Func<int, int, Task> LatencyUpdated;

    /// <summary>
    ///     Occurs when the latency of the voice UDP server has been updated.
    ///     The first <see cref="int"/> parameter is the latency in milliseconds before the update.
    ///     The second <see cref="int"/> parameter is the current updated latency in milliseconds.
    /// </summary>
    event Func<int, int, Task> UdpLatencyUpdated;

    /// <summary>
    ///     Gets the current connection state of this client.
    /// </summary>
    ConnectionState ConnectionState { get; }

    // /// <summary>
    // ///     Gets the estimated round-trip latency, in milliseconds, to the voice WebSocket server.
    // /// </summary>
    // int Latency { get; }

    /// <summary>
    ///     Gets the estimated round-trip latency, in milliseconds, to the voice UDP server.
    /// </summary>
    int UdpLatency { get; }

    /// <summary>
    ///     Stops the client from sending audio.
    /// </summary>
    /// <returns> A task representing the asynchronous operation. </returns>
    Task StopAsync();

    /// <summary>
    ///     Creates a new audio stream from the Opus codec.
    /// </summary>
    /// <param name="bufferMillis"> The buffer size, in milliseconds, of the audio stream. </param>
    /// <returns> A new Opus audio stream. </returns>
    AudioOutStream CreateOpusStream(int bufferMillis = 1000);

    /// <summary>
    ///     Creates a new audio stream from the Opus codec without buffering.
    /// </summary>
    /// <returns> A new Opus audio stream. </returns>
    AudioOutStream CreateDirectOpusStream();

    /// <summary>
    ///     Creates a new audio stream from the PCM codec.
    /// </summary>
    /// <param name="application"> The audio application to use. </param>
    /// <param name="bitrate"> The bitrate of the audio stream; leave null to use the bitrate requested from KOOK. </param>
    /// <param name="bufferMillis"> The buffer size, in milliseconds, of the audio stream. </param>
    /// <param name="packetLoss"> The packet loss percentage of the audio stream. </param>
    /// <returns> A new PCM audio stream. </returns>
    AudioOutStream CreatePcmStream(AudioApplication application, int? bitrate = null, int bufferMillis = 1000, int packetLoss = 30);

    /// <summary>
    ///     Creates a new audio stream from the PCM codec without buffering.
    /// </summary>
    /// <param name="application"> The audio application to use. </param>
    /// <param name="bitrate"> The bitrate of the audio stream; leave null to use the bitrate requested from KOOK. </param>
    /// <param name="packetLoss"> The packet loss percentage of the audio stream. </param>
    /// <returns> A new PCM audio stream. </returns>
    AudioOutStream CreateDirectPcmStream(AudioApplication application, int? bitrate = null, int packetLoss = 30);
}
