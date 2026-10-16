using System;
using System.Collections.Generic;
using System.Text.Json;
using Kook.Audio.WebRtc.Signaling;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests that the signaling models can be deserialized from payload shapes observed on a real voice channel.
///     All identifiers, credentials and fingerprints in the samples are placeholders.
/// </summary>
[Trait("Category", "Unit")]
public class VoiceSignalingModelsTests
{
    private const string QuickJoinResponseJson = """
        {
          "peerId": "1000001",
          "pTransport": {
            "id": "11111111-1111-1111-1111-111111111111",
            "iceParameters": { "iceLite": true, "usernameFragment": "ufrag-p", "password": "pwd-p" },
            "iceCandidates": [
              { "foundation": "udpcandidate", "ip": "10.0.0.1", "port": 40078, "priority": 1076225279, "protocol": "udp", "type": "host" },
              { "foundation": "udpcandidate", "ip": "203.0.113.5", "port": 40079, "priority": 1076199679, "protocol": "udp", "type": "host" }
            ],
            "dtlsParameters": {
              "fingerprints": [
                { "algorithm": "sha-1", "value": "AA:BB" },
                { "algorithm": "sha-256", "value": "CC:DD" }
              ],
              "role": "auto"
            },
            "sctpParameters": {}
          },
          "cTransport": {
            "id": "22222222-2222-2222-2222-222222222222",
            "iceParameters": { "iceLite": true, "usernameFragment": "ufrag-c", "password": "pwd-c" },
            "iceCandidates": [
              { "foundation": "udpcandidate", "ip": "203.0.113.5", "port": 40079, "priority": 1076199679, "protocol": "udp", "type": "host" }
            ],
            "dtlsParameters": { "fingerprints": [ { "algorithm": "sha-256", "value": "CC:DD" } ], "role": "auto" },
            "sctpParameters": {}
          },
          "rtpConfig": { "audio": { "stereo": 0, "ptime": 60, "maxaveragebitrate": 32000, "usedtx": 1 }, "video": {} }
        }
        """;

    private const string InitSpeakingResponseJson = """
        {
          "peers": [
            { "id": "2000002", "peerId": "2000002", "roomId": "3000003", "displayName": "user",
              "username": "u", "identify_num": "0439", "avatar": "https://example.invalid/a.png", "vip_avatar": "https://example.invalid/a.png",
              "is_vip": true, "device": "pc", "pausedProducer": true, "pausedHeadset": false, "accompaniment": false,
              "permissionVoiceManage": true, "permissionFreeMic": true, "permissionSay": true,
              "roles": [367092, 51590363], "isServerMute": false,
              "extra": { "id_level": 2, "is_manager": true },
              "someFutureField": 1 }
          ],
          "room": {
            "roomId": "3000003", "mode": 0, "speakList": [], "applyList": [], "inviteList": [],
            "speakTime": 60000, "applyTimeout": 120000, "inviteTimeout": 60000, "maxSpeakers": 16,
            "seatList": [ { "seatId": 0, "pauseMic": false }, { "seatId": 1, "pauseMic": true } ]
          },
          "peer": { "id": "1000001", "peerId": "1000001", "device": "bot", "roles": [76878434] },
          "producer": { "id": "44444444-4444-4444-4444-444444444444" }
        }
        """;

    private const string NewConsumerJson = """
        {
          "peerId": "2000002",
          "producerId": "55555555-5555-5555-5555-555555555555",
          "id": "66666666-6666-6666-6666-666666666666",
          "kind": "audio",
          "type": "simple",
          "rtpParameters": {
            "codecs": [ { "mimeType": "audio/opus", "payloadType": 100, "clockRate": 48000, "channels": 2,
                          "parameters": { "minptime": 10, "useinbandfec": 1, "sprop-stereo": 0, "maxaveragebitrate": 32000 },
                          "rtcpFeedback": [ { "type": "nack", "parameter": "" } ] } ],
            "encodings": [ { "ssrc": 550059438 } ],
            "rtcp": { "cname": "c", "reducedSize": true, "mux": true },
            "mid": "0"
          },
          "appData": { "peerId": "2000002", "isBot": false, "isVip": false, "transportId": "77777777-7777-7777-7777-777777777777",
                       "peerInfo": { "id": "2000002", "peerId": "2000002", "device": "android" } },
          "producerPaused": false
        }
        """;

    private static T Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, ProtooJson.Options)!;

    [Fact]
    public void QuickJoinResponseParses()
    {
        QuickJoinResponse response = Deserialize<QuickJoinResponse>(QuickJoinResponseJson);

        Assert.Equal("1000001", response.PeerId);
        Assert.True(response.ProducerTransport.IceParameters.IceLite);
        Assert.Equal(2, response.ProducerTransport.IceCandidates.Count);
        Assert.Equal("203.0.113.5", response.ProducerTransport.IceCandidates[1].Ip);
        Assert.Equal(40079, response.ProducerTransport.IceCandidates[1].Port);
        Assert.Equal(1076199679u, response.ProducerTransport.IceCandidates[1].Priority);
        Assert.Equal("host", response.ProducerTransport.IceCandidates[1].Type);
        Assert.Equal(2, response.ProducerTransport.DtlsParameters.Fingerprints.Count);
        Assert.Equal("auto", response.ProducerTransport.DtlsParameters.Role);
        Assert.Equal("22222222-2222-2222-2222-222222222222", response.ConsumerTransport.Id);
        Assert.Equal(60, response.RtpConfig!.Audio!.PacketTime);
        Assert.Equal(32000, response.RtpConfig.Audio.MaxAverageBitrate);
        Assert.Equal(1, response.RtpConfig.Audio.UseDtx);
        Assert.Equal(0, response.RtpConfig.Audio.Stereo);
    }

    [Fact]
    public void QuickJoinResponseWithoutTransportsIsRejected() =>
        Assert.Throws<JsonException>(() => Deserialize<QuickJoinResponse>("""{"peerId":"1"}"""));

    [Fact]
    public void InitSpeakingResponseParses()
    {
        InitSpeakingResponse response = Deserialize<InitSpeakingResponse>(InitSpeakingResponseJson);

        Assert.Equal("44444444-4444-4444-4444-444444444444", response.Producer!.Id);
        Assert.Equal("3000003", response.Room!.RoomId);
        Assert.Equal(0, response.Room.Mode);
        Assert.Equal(16, response.Room.MaxSpeakers);
        Assert.Equal(2, response.Room.SeatList!.Count);
        Assert.True(response.Room.SeatList[1].PauseMic);
        Assert.Empty(response.Room.SpeakList!);
        Assert.Equal("bot", response.Peer!.Device);
        VoicePeerInfo peer = Assert.Single(response.Peers!);
        Assert.Equal("2000002", peer.PeerId);
        Assert.Equal("pc", peer.Device);
        Assert.True(peer.PausedProducer);
        Assert.Equal("0439", peer.IdentifyNumber);
        Assert.Equal(new ulong[] { 367092, 51590363 }, peer.Roles);
        Assert.True(peer.ExtraData!.ContainsKey("someFutureField"));
    }

    [Fact]
    public void NewConsumerParses()
    {
        NewConsumerData data = Deserialize<NewConsumerData>(NewConsumerJson);

        Assert.Equal("2000002", data.PeerId);
        Assert.Equal("66666666-6666-6666-6666-666666666666", data.Id);
        Assert.Equal("audio", data.Kind);
        Assert.False(data.ProducerPaused);
        Assert.False(data.AppData!.IsBot);
        Assert.Equal("android", data.AppData.PeerInfo!.Device);
        JsonElement rtp = data.RtpParameters;
        Assert.Equal(550059438u, rtp.GetProperty("encodings")[0].GetProperty("ssrc").GetUInt32());
        Assert.Equal(100, rtp.GetProperty("codecs")[0].GetProperty("payloadType").GetInt32());
    }

    [Fact]
    public void TurnTransportResponseParses()
    {
        TurnTransportResponse response = Deserialize<TurnTransportResponse>(
            """{"username":"u","password":"p","urls":["turns:turn.example.invalid:443?transport=tcp"]}""");

        Assert.Equal("turns:turn.example.invalid:443?transport=tcp", Assert.Single(response.Urls));
        Assert.Equal("u", response.Username);
    }

    [Fact]
    public void DisconnectDataParses()
    {
        VoiceDisconnectData data = Deserialize<VoiceDisconnectData>(
            """{"errorCode":41106,"errorReason":"你已被管理员移出当前语音频道。"}""");

        Assert.Equal(41106, data.ErrorCode);
        Assert.Equal("你已被管理员移出当前语音频道。", data.ErrorReason);
    }

    [Fact]
    public void InitSpeakingParamsSerializeToObservedShape()
    {
        InitSpeakingParams args = new()
        {
            PauseMic = true,
            RtpParameters = JsonDocument.Parse("""{"mid":"0","codecs":[]}""").RootElement.Clone(),
            ProducerDtls = new VoiceDtlsParameters
            {
                Role = "client",
                Fingerprints = [new VoiceDtlsFingerprint { Algorithm = "sha-256", Value = "AA" }]
            },
            ConsumerDtls = new VoiceDtlsParameters
            {
                Role = "client",
                Fingerprints = [new VoiceDtlsFingerprint { Algorithm = "sha-256", Value = "BB" }]
            }
        };

        using JsonDocument document = JsonDocument.Parse(JsonSerializer.Serialize(args, ProtooJson.Options));
        JsonElement root = document.RootElement;
        Assert.True(root.GetProperty("pauseMic").GetBoolean());
        Assert.False(root.GetProperty("pauseHeadSet").GetBoolean());
        Assert.Equal("0", root.GetProperty("rtpParameters").GetProperty("mid").GetString());
        Assert.Equal("AA", root.GetProperty("pDtls").GetProperty("fingerprints")[0].GetProperty("value").GetString());
        Assert.Equal("BB", root.GetProperty("cDtls").GetProperty("fingerprints")[0].GetProperty("value").GetString());
    }
}
