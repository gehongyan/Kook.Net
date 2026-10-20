using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Kook.Audio.WebRtc.Signaling;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="VoiceSignalingClient"/>.
/// </summary>
[Trait("Category", "Unit")]
public class VoiceSignalingClientTests
{
    private static readonly TimeSpan LongInterval = TimeSpan.FromHours(1);

    private static (VoiceSignalingClient Client, FakeProtooTransport Transport) CreateClient()
    {
        FakeProtooTransport transport = new();
        VoiceSignalingClient client = new(new ProtooPeer(transport, heartbeatInterval: LongInterval));
        return (client, transport);
    }

    private static async Task<ProtooRequest> WaitForRequestAsync(FakeProtooTransport transport, string method)
    {
        string frame = await transport.WaitForSentAsync(text =>
            ProtooMessageCodec.Decode(text) is ProtooRequest request && request.Method == method);
        return (ProtooRequest)ProtooMessageCodec.Decode(frame)!;
    }

    [Fact]
    public async Task ConnectUsesTheGatewayUrlOfTheToken()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoiceGatewayToken token = new("wss://example.invalid/gateway/v1?token=secret", null, null);

        await client.ConnectAsync(token);

        Assert.Equal("protoo", transport.SubProtocol);
        Assert.Equal("wss://example.invalid/gateway/v1?token=secret", transport.ConnectedUri!.ToString());
    }

    [Fact]
    public async Task QuickJoinSendsExpectedRequestAndParsesResponse()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;

        Task<QuickJoinResponse> pending = client.QuickJoinAsync(new QuickJoinParams { ForceRelay = false });
        ProtooRequest request = await WaitForRequestAsync(transport, "quickJoin");
        Assert.Equal("webrtc", request.Data!.Value.GetProperty("transport").GetString());
        Assert.False(request.Data!.Value.GetProperty("forceTcp").GetBoolean());
        Assert.False(request.Data!.Value.GetProperty("forceRelay").GetBoolean());

        await transport.ReceiveAsync($$$"""
            {"response":true,"id":{{{request.Id}}},"ok":true,"data":{
              "peerId":"1",
              "pTransport":{"id":"p","iceParameters":{"usernameFragment":"u","password":"w"},"iceCandidates":[],"dtlsParameters":{"fingerprints":[]}},
              "cTransport":{"id":"c","iceParameters":{"usernameFragment":"u","password":"w"},"iceCandidates":[],"dtlsParameters":{"fingerprints":[]}}
            }}
            """);

        QuickJoinResponse response = await pending;
        Assert.Equal("p", response.ProducerTransport.Id);
        Assert.Equal("c", response.ConsumerTransport.Id);
    }

    [Fact]
    public async Task ProducerRequestsCarryTheProducerId()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;

        foreach ((string method, Func<string, Task> call) in new (string, Func<string, Task>)[]
                 {
                     ("pauseProducer", id => client.PauseProducerAsync(id)),
                     ("resumeProducer", id => client.ResumeProducerAsync(id)),
                     ("closeProducer", id => client.CloseProducerAsync(id))
                 })
        {
            Task pending = call("producer-1");
            ProtooRequest request = await WaitForRequestAsync(transport, method);
            Assert.Equal("producer-1", request.Data!.Value.GetProperty("producerId").GetString());
            await transport.ReceiveAsync($$$"""{"response":true,"id":{{{request.Id}}},"ok":true,"data":{}}""");
            await pending;
        }
    }

    [Fact]
    public async Task NewConsumerIsPassedToTheHandlerAndAccepted()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        NewConsumerData? received = null;
        client.NewConsumerHandler = data =>
        {
            received = data;
            return Task.FromResult(ProtooRequestResult.Accept());
        };

        await transport.ReceiveAsync("""
            {"request":true,"id":11,"method":"newConsumer","data":{
              "peerId":"2","producerId":"pr","id":"co","kind":"audio","rtpParameters":{"encodings":[{"ssrc":1}]}}}
            """);
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.Equal(11, response.Id);
        Assert.True(response.Ok);
        Assert.Equal("co", received!.Id);
        Assert.Equal("2", received.PeerId);
    }

    [Fact]
    public async Task NewConsumerIsRejectedWithoutAHandler()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;

        await transport.ReceiveAsync("""
            {"request":true,"id":12,"method":"newConsumer","data":{
              "peerId":"2","producerId":"pr","id":"co","kind":"audio","rtpParameters":{}}}
            """);
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.False(response.Ok);
        Assert.Equal(503, response.ErrorCode);
    }

    [Fact]
    public async Task NewConsumerWithInvalidDataIsRejectedWith400()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        client.NewConsumerHandler = _ => Task.FromResult(ProtooRequestResult.Accept());

        await transport.ReceiveAsync("""{"request":true,"id":13,"method":"newConsumer"}""");
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.False(response.Ok);
        Assert.Equal(400, response.ErrorCode);
    }

    [Fact]
    public async Task ActiveSpeakerRequestIsAccepted()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;

        await transport.ReceiveAsync("""{"request":true,"id":14,"method":"active-speaker","data":{"peerName":"x"}}""");
        string frame = await transport.WaitForSentAsync();

        Assert.True(Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame)).Ok);
    }

    [Fact]
    public async Task UnknownServerRequestIsRejectedWith404()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;

        await transport.ReceiveAsync("""{"request":true,"id":15,"method":"somethingNew"}""");
        string frame = await transport.WaitForSentAsync();

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame));
        Assert.False(response.Ok);
        Assert.Equal(404, response.ErrorCode);
    }

    [Fact]
    public async Task CloseByOtherDeviceRaisesEventAndIsAccepted()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        TaskCompletionSource<bool> raised = new();
        client.CloseByOtherDevice += () =>
        {
            raised.TrySetResult(true);
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync("""{"request":true,"id":16,"method":"closeByOtherDevice"}""");
        string frame = await transport.WaitForSentAsync();

        Assert.True(await raised.Task);
        Assert.True(Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame)).Ok);
    }

    [Fact]
    public async Task DisconnectNotificationRaisesTypedEventAndRawNotification()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoiceDisconnectData? disconnect = null;
        ProtooNotification? raw = null;
        client.Disconnected += data =>
        {
            disconnect = data;
            return Task.CompletedTask;
        };
        client.NotificationReceived += notification =>
        {
            raw = notification;
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(
            """{"notification":true,"method":"disconnect","data":{"errorCode":41106,"errorReason":"你已被管理员移出当前语音频道。"}}""");

        Assert.Equal(41106, disconnect!.ErrorCode);
        Assert.Equal("你已被管理员移出当前语音频道。", disconnect.ErrorReason);
        Assert.Equal("disconnect", raw!.Method);
    }

    [Fact]
    public async Task StateNotificationsAfterPeerClosedAreDeliveredWithoutError()
    {
        // 实测：状态类通知可能晚于 peerClosed 到达，信令层应当原样转发，由上层决定忽略
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        List<string> methods = [];
        client.NotificationReceived += notification =>
        {
            methods.Add(notification.Method);
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync("""{"notification":true,"method":"peerClosed","data":{"peerId":"2","fromId":"3"}}""");
        await transport.ReceiveAsync("""{"notification":true,"method":"consumerPaused","data":{"peerId":"2"}}""");

        Assert.Equal(["peerClosed", "consumerPaused"], methods);
    }

    [Fact]
    public async Task ClosedEventCarriesTheWebSocketCloseCode()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        ProtooCloseInfo? info = null;
        client.Closed += closeInfo =>
        {
            info = closeInfo;
            return Task.CompletedTask;
        };

        await transport.CloseFromServerAsync(4000, "terminated");

        Assert.Equal(4000, info!.Value.Code);
        Assert.Equal("terminated", info.Value.Reason);
        Assert.True(client.IsClosed);
    }
}
