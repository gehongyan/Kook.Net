using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kook.Audio.WebRtc.Signaling;
using Kook.Logging;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for the classified notifications of <see cref="VoiceSignalingClient"/>.
/// </summary>
[Trait("Category", "Unit")]
public class VoiceSignalingNotificationTests
{
    private static readonly TimeSpan LongInterval = TimeSpan.FromHours(1);

    private static (VoiceSignalingClient Client, FakeProtooTransport Transport) CreateClient(Logger? logger = null)
    {
        FakeProtooTransport transport = new();
        VoiceSignalingClient client = new(new ProtooPeer(transport, heartbeatInterval: LongInterval), logger);
        return (client, transport);
    }

    private static string Notification(string method, string? dataJson = null) =>
        dataJson is null
            ? $$"""{"notification":true,"method":"{{method}}"}"""
            : $$"""{"notification":true,"method":"{{method}}","data":{{dataJson}}}""";

    [Fact]
    public async Task NewPeerRaisesTheEventWithThePeerInfo()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoicePeerInfo? peer = null;
        client.NewPeer += info =>
        {
            peer = info;
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(Notification("newPeer", """
            {"id":"2810246202","peerId":"2810246202","roomId":"3237184048323209","displayName":"someone","device":"android",
             "pausedProducer":true,"pausedHeadset":false,"permissionSay":true}
            """));

        Assert.NotNull(peer);
        Assert.Equal("2810246202", peer.PeerId);
        Assert.Equal("someone", peer.DisplayName);
        Assert.Equal("android", peer.Device);
        Assert.True(peer.PausedProducer);
        Assert.False(peer.PausedHeadset);
    }

    [Fact]
    public async Task PeerClosedRaisesTheEventWithThePeerId()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoicePeerRefData? closed = null;
        client.PeerClosed += data =>
        {
            closed = data;
            return Task.CompletedTask;
        };

        // fromId 在实测中等于频道 ID，多余的字段应当被忽略
        await transport.ReceiveAsync(Notification("peerClosed", """{"peerId":"2810246202","fromId":"3237184048323209"}"""));

        Assert.Equal("2810246202", closed!.PeerId);
    }

    [Fact]
    public async Task ConsumerClosedRaisesTheEventWithTheConsumerId()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoiceConsumerRefData? closed = null;
        client.ConsumerClosed += data =>
        {
            closed = data;
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(Notification("consumerClosed", """{"consumerId":"c-1"}"""));

        Assert.Equal("c-1", closed!.ConsumerId);
    }

    [Theory]
    [InlineData("consumerPaused")]
    [InlineData("consumerResumed")]
    [InlineData("pauseHeadset")]
    [InlineData("resumeHeadset")]
    [InlineData("stopAccompaniment")]
    public async Task PeerReferenceNotificationsRaiseOnlyTheirOwnEvent(string method)
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        List<(string Event, string? PeerId)> raised = [];
        Task Record(string name, VoicePeerRefData data)
        {
            raised.Add((name, data.PeerId));
            return Task.CompletedTask;
        }

        client.ConsumerPaused += data => Record("consumerPaused", data);
        client.ConsumerResumed += data => Record("consumerResumed", data);
        client.PauseHeadset += data => Record("pauseHeadset", data);
        client.ResumeHeadset += data => Record("resumeHeadset", data);
        client.StopAccompaniment += data => Record("stopAccompaniment", data);

        // consumerPaused / consumerResumed 在实测中不带 consumerId，只有 peerId
        await transport.ReceiveAsync(Notification(method, """{"peerId":"821393881"}"""));

        (string Event, string? PeerId) only = Assert.Single(raised);
        Assert.Equal(method, only.Event);
        Assert.Equal("821393881", only.PeerId);
    }

    [Fact]
    public async Task PeerPermissionChangedRaisesTheEventWithAllFields()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoicePeerPermissionChangedData? changed = null;
        client.PeerPermissionChanged += data =>
        {
            changed = data;
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(Notification("peerPermissionChanged", """
            {"peerId":"1462506463","roomId":"3237184048323209","isServerMute":false,"isServerMuteHeadset":true,
             "permissionFreeMic":true,"permissionLink":false,"permissionSay":true,"permissionVoiceManage":false}
            """));

        Assert.NotNull(changed);
        Assert.Equal("1462506463", changed.PeerId);
        Assert.False(changed.IsServerMute);
        Assert.True(changed.IsServerMuteHeadset);
        Assert.True(changed.PermissionFreeMic);
        Assert.False(changed.PermissionLink);
        Assert.True(changed.PermissionSay);
        Assert.False(changed.PermissionVoiceManage);
    }

    [Fact]
    public async Task PermissionFieldsThatAreNotSentStayNull()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoicePeerPermissionChangedData? changed = null;
        client.PeerPermissionChanged += data =>
        {
            changed = data;
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(Notification("peerPermissionChanged", """{"peerId":"1","permissionLink":false}"""));

        // 没有下发的字段不能被当成 false
        Assert.False(changed!.PermissionLink);
        Assert.Null(changed.PermissionSay);
        Assert.Null(changed.IsServerMute);
    }

    [Fact]
    public async Task AtmosphereRaisesTheEvent()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoiceAtmosphereData? atmosphere = null;
        client.Atmosphere += data =>
        {
            atmosphere = data;
            return Task.CompletedTask;
        };

        // 实测的载荷：itemId 是整数，不是字符串
        await transport.ReceiveAsync(Notification("atmosphere", """{"peerId":"2810246202","itemId":11065}"""));

        Assert.Equal("2810246202", atmosphere!.PeerId);
        Assert.Equal(11065UL, atmosphere.ItemId);
    }

    [Fact]
    public async Task StartAccompanimentRaisesTheEventWithTheMusicInfo()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        VoiceAccompanimentData? accompaniment = null;
        client.StartAccompaniment += data =>
        {
            accompaniment = data;
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(Notification("startAccompaniment", """
            {"peerId":"821393881","data":{"music_name":"song","music_software":"player","singer":"someone"}}
            """));

        Assert.Equal("821393881", accompaniment!.PeerId);
        Assert.Equal("song", accompaniment.Data!.MusicName);
        Assert.Equal("player", accompaniment.Data.MusicSoftware);
        Assert.Equal("someone", accompaniment.Data.Singer);
    }

    [Fact]
    public async Task ActiveSpeakerRequestRaisesTheEventWithTheRawData()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        string? rawData = null;
        client.ActiveSpeaker += data =>
        {
            rawData = data?.GetRawText();
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync("""{"request":true,"id":21,"method":"active-speaker","data":{"peerName":"x"}}""");
        string frame = await transport.WaitForSentAsync();

        Assert.Equal("""{"peerName":"x"}""", rawData);
        Assert.True(Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame)).Ok);
    }

    [Fact]
    public async Task ReloadRequestIsAcceptedBeforeTheEventIsRaised()
    {
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient();
        using VoiceSignalingClient _ = client;
        TaskCompletionSource<bool> raised = new();
        client.ReloadRequested += () =>
        {
            raised.TrySetResult(true);
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync("""{"request":true,"id":22,"method":"reload"}""");
        string frame = await transport.WaitForSentAsync();

        Assert.True(Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(frame)).Ok);
        Assert.True(await raised.Task);
    }

    [Theory]
    [InlineData("newPeer", "\"not an object\"")]
    [InlineData("peerClosed", "[1,2,3]")]
    [InlineData("consumerClosed", null)]
    [InlineData("peerPermissionChanged", "{\"permissionSay\":\"maybe\"}")]
    [InlineData("atmosphere", "{\"peerId\":\"1\",\"itemId\":\"abc\"}")]
    public async Task MalformedPayloadsAreLoggedAsWarningsAndDoNotRaiseEvents(string method, string? dataJson)
    {
        LogManager logManager = new(LogSeverity.Debug);
        List<LogMessage> logs = [];
        logManager.Message += message =>
        {
            logs.Add(message);
            return Task.CompletedTask;
        };
        (VoiceSignalingClient client, FakeProtooTransport transport) = CreateClient(logManager.CreateLogger("test"));
        using VoiceSignalingClient _ = client;
        int raisedCount = 0;
        Task Count<T>(T _)
        {
            raisedCount++;
            return Task.CompletedTask;
        }

        client.NewPeer += Count;
        client.PeerClosed += Count;
        client.ConsumerClosed += Count;
        client.PeerPermissionChanged += Count;
        client.Atmosphere += Count;
        List<string> rawMethods = [];
        client.NotificationReceived += notification =>
        {
            rawMethods.Add(notification.Method);
            return Task.CompletedTask;
        };

        await transport.ReceiveAsync(Notification(method, dataJson));

        // 载荷无法解析的通知不应当引发分类事件，也不应当让信令连接出错，但必须输出警告日志，不能静默丢弃
        Assert.Equal(0, raisedCount);
        Assert.Equal([method], rawMethods);
        Assert.False(client.IsClosed);
        LogMessage warning = Assert.Single(logs, x => x.Severity == LogSeverity.Warning);
        Assert.Contains(method, warning.Message);
    }
}
