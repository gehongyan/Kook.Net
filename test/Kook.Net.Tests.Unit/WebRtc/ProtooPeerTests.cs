using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Kook.Audio.WebRtc.Signaling;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="ProtooPeer"/>.
/// </summary>
[Trait("Category", "Unit")]
public class ProtooPeerTests
{
    private static readonly TimeSpan LongInterval = TimeSpan.FromHours(1);

    private static async Task<long> WaitForRequestIdAsync(FakeProtooTransport transport, string method)
    {
        string frame = await transport.WaitForSentAsync(text =>
            ProtooMessageCodec.Decode(text) is ProtooRequest request && request.Method == method);
        return ((ProtooRequest)ProtooMessageCodec.Decode(frame)!).Id;
    }

    [Fact]
    public async Task ConnectUsesProtooSubProtocol()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        await peer.ConnectAsync(new Uri("wss://example.invalid/gateway/v1"));

        Assert.Equal("protoo", transport.SubProtocol);
        Assert.Equal("wss://example.invalid/gateway/v1", transport.ConnectedUri!.ToString());
    }

    [Fact]
    public async Task RequestReturnsResponseData()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        Task<JsonElement?> pending = peer.RequestAsync("pauseProducer", new ProducerParams { ProducerId = "p1" });
        long id = await WaitForRequestIdAsync(transport, "pauseProducer");
        await transport.ReceiveAsync($$$"""{"response":true,"id":{{{id}}},"ok":true,"data":{"x":"y"}}""");

        JsonElement? data = await pending;
        Assert.Equal("y", data!.Value.GetProperty("x").GetString());
    }

    [Fact]
    public async Task RequestSendsProducerIdInData()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        _ = peer.RequestAsync("closeProducer", new ProducerParams { ProducerId = "abc" });
        string frame = await transport.WaitForSentAsync();

        ProtooRequest request = Assert.IsType<ProtooRequest>(ProtooMessageCodec.Decode(frame));
        Assert.Equal("abc", request.Data!.Value.GetProperty("producerId").GetString());
    }

    [Fact]
    public async Task RequestThrowsOnErrorResponse()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        Task<JsonElement?> pending = peer.RequestAsync("initSpeaking");
        long id = await WaitForRequestIdAsync(transport, "initSpeaking");
        await transport.ReceiveAsync($$"""{"response":true,"id":{{id}},"ok":false,"errorCode":403,"errorReason":"denied"}""");

        ProtooException exception = await Assert.ThrowsAsync<ProtooException>(() => pending);
        Assert.Equal(403, exception.ErrorCode);
        Assert.Equal("denied", exception.ErrorReason);
    }

    [Fact]
    public async Task RequestTimesOut()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        await Assert.ThrowsAsync<TimeoutException>(() =>
            peer.RequestAsync("quickJoin", timeout: TimeSpan.FromMilliseconds(50)));
    }

    [Fact]
    public async Task RequestHonorsCancellation()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        using CancellationTokenSource cts = new();

        Task<JsonElement?> pending = peer.RequestAsync("quickJoin", cancellationToken: cts.Token);
        await WaitForRequestIdAsync(transport, "quickJoin");
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => pending);
    }

    [Fact]
    public async Task ResponseForUnknownRequestIsIgnored()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        await transport.ReceiveAsync("""{"response":true,"id":999,"ok":true,"data":{}}""");

        Assert.False(peer.IsClosed);
    }

    [Fact]
    public async Task IncomingRequestIsAnsweredByHandler()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        peer.RequestHandler = request =>
            Task.FromResult(ProtooRequestResult.Accept(new { method = request.Method }));

        await transport.ReceiveAsync("""{"request":true,"id":77,"method":"newConsumer","data":{}}""");
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.Equal(77, response.Id);
        Assert.True(response.Ok);
        Assert.Equal("newConsumer", response.Data!.Value.GetProperty("method").GetString());
    }

    [Fact]
    public async Task IncomingRequestWithoutHandlerIsRejectedWith404()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);

        await transport.ReceiveAsync("""{"request":true,"id":5,"method":"whatever"}""");
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.Equal(5, response.Id);
        Assert.False(response.Ok);
        Assert.Equal(404, response.ErrorCode);
    }

    [Fact]
    public async Task IncomingRequestThatThrowsIsRejectedWith500()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        peer.RequestHandler = _ => throw new InvalidOperationException("boom");

        await transport.ReceiveAsync("""{"request":true,"id":6,"method":"newConsumer"}""");
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.False(response.Ok);
        Assert.Equal(500, response.ErrorCode);
        Assert.Equal("boom", response.ErrorReason);
    }

    [Fact]
    public async Task NotificationRaisesEvent()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        TaskCompletionSource<ProtooNotification> received = new();
        peer.NotificationReceived += notification =>
        {
            received.TrySetResult(notification);
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync("""{"notification":true,"method":"consumerPaused","data":{"peerId":"1"}}""");

        ProtooNotification notification = await received.Task;
        Assert.Equal("consumerPaused", notification.Method);
        Assert.Equal("1", notification.Data!.Value.GetProperty("peerId").GetString());
    }

    [Fact]
    public async Task ThrowingNotificationHandlerDoesNotBreakTheReceiveLoop()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        peer.NotificationReceived += _ => throw new InvalidOperationException("boom");

        await transport.ReceiveAsync("""{"notification":true,"method":"newPeer"}""");

        Assert.False(peer.IsClosed);
    }

    [Fact]
    public async Task CloseFailsPendingRequestsAndRaisesClosedOnce()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        int closedCount = 0;
        ProtooCloseInfo? closeInfo = null;
        peer.Closed += info =>
        {
            Interlocked.Increment(ref closedCount);
            closeInfo = info;
            return Task.CompletedTask;
        };

        Task<JsonElement?> pending = peer.RequestAsync("quickJoin");
        await WaitForRequestIdAsync(transport, "quickJoin");
        await transport.CloseFromServerAsync(4000, "bye");
        await transport.CloseFromServerAsync(4000, "bye");

        ProtooException exception = await Assert.ThrowsAsync<ProtooException>(() => pending);
        Assert.Equal(4000, exception.ErrorCode);
        Assert.Equal(1, closedCount);
        Assert.Equal(4000, closeInfo!.Value.Code);
        Assert.True(peer.IsClosed);
    }

    [Fact]
    public async Task RequestAfterCloseThrows()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: LongInterval);
        await transport.CloseFromServerAsync(1006, "abnormal");

        await Assert.ThrowsAsync<ProtooException>(() => peer.RequestAsync("quickJoin"));
    }

    [Fact]
    public async Task HeartbeatSendsPlainTextPing()
    {
        FakeProtooTransport transport = new();
        using ProtooPeer peer = new(transport, heartbeatInterval: TimeSpan.FromMilliseconds(30));

        await peer.ConnectAsync(new Uri("wss://example.invalid/"));
        string frame = await transport.WaitForSentAsync(text => text == "ping");

        // 心跳是纯文本，不是 protoo 的 JSON 帧
        Assert.Equal("ping", frame);
        Assert.Null(ProtooMessageCodec.Decode(frame));
    }
}
