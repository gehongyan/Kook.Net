using System;
using System.Text.Json;
using Kook.Audio.WebRtc.Signaling;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="ProtooMessageCodec"/>.
/// </summary>
[Trait("Category", "Unit")]
public class ProtooMessageCodecTests
{
    [Fact]
    public void DecodesRequest()
    {
        ProtooMessage? message = ProtooMessageCodec.Decode(
            """{"request":true,"id":42,"method":"newConsumer","data":{"a":1}}""");

        ProtooRequest request = Assert.IsType<ProtooRequest>(message);
        Assert.Equal(42, request.Id);
        Assert.Equal("newConsumer", request.Method);
        Assert.Equal(1, request.Data!.Value.GetProperty("a").GetInt32());
    }

    [Fact]
    public void DecodesSuccessResponse()
    {
        ProtooMessage? message = ProtooMessageCodec.Decode(
            """{"response":true,"id":7,"ok":true,"data":{"producer":{"id":"p"}}}""");

        ProtooResponse response = Assert.IsType<ProtooResponse>(message);
        Assert.Equal(7, response.Id);
        Assert.True(response.Ok);
        Assert.Equal("p", response.Data!.Value.GetProperty("producer").GetProperty("id").GetString());
        Assert.Null(response.ErrorCode);
    }

    [Fact]
    public void DecodesEmptyDataObjectResponse()
    {
        // pauseProducer、resumeProducer、closeProducer 的实测响应就是空对象
        ProtooResponse response = Assert.IsType<ProtooResponse>(
            ProtooMessageCodec.Decode("""{"response":true,"id":1,"ok":true,"data":{}}"""));

        Assert.True(response.Ok);
        Assert.NotNull(response.Data);
        Assert.Equal(JsonValueKind.Object, response.Data!.Value.ValueKind);
    }

    [Fact]
    public void DecodesErrorResponse()
    {
        ProtooMessage? message = ProtooMessageCodec.Decode(
            """{"response":true,"id":9,"ok":false,"errorCode":404,"errorReason":"not found"}""");

        ProtooResponse response = Assert.IsType<ProtooResponse>(message);
        Assert.False(response.Ok);
        Assert.Equal(404, response.ErrorCode);
        Assert.Equal("not found", response.ErrorReason);
        Assert.Null(response.Data);
    }

    [Fact]
    public void DecodesNotification()
    {
        ProtooMessage? message = ProtooMessageCodec.Decode(
            """{"notification":true,"method":"peerClosed","data":{"peerId":"1","fromId":"2"}}""");

        ProtooNotification notification = Assert.IsType<ProtooNotification>(message);
        Assert.Equal("peerClosed", notification.Method);
        Assert.Equal("1", notification.Data!.Value.GetProperty("peerId").GetString());
    }

    [Fact]
    public void DecodesIdSentAsString()
    {
        ProtooRequest request = Assert.IsType<ProtooRequest>(
            ProtooMessageCodec.Decode("""{"request":true,"id":"123","method":"m"}"""));

        Assert.Equal(123, request.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("pong")]
    [InlineData("[]")]
    [InlineData("123")]
    [InlineData("""{"foo":1}""")]
    [InlineData("""{"request":true,"id":1}""")] // 缺少 method
    [InlineData("""{"notification":true}""")] // 缺少 method
    [InlineData("""{"response":true,"ok":true}""")] // 缺少 id
    public void IgnoresFramesThatAreNotProtooMessages(string text) =>
        Assert.Null(ProtooMessageCodec.Decode(text));

    [Fact]
    public void EncodedRequestRoundTrips()
    {
        string text = ProtooMessageCodec.EncodeRequest(5, "quickJoin", new QuickJoinParams { ForceRelay = false });

        ProtooRequest request = Assert.IsType<ProtooRequest>(ProtooMessageCodec.Decode(text));
        Assert.Equal(5, request.Id);
        Assert.Equal("quickJoin", request.Method);
        Assert.Equal("webrtc", request.Data!.Value.GetProperty("transport").GetString());
        Assert.False(request.Data!.Value.GetProperty("forceRelay").GetBoolean());
    }

    [Fact]
    public void NullPropertiesAreOmittedFromEncodedData()
    {
        string text = ProtooMessageCodec.EncodeRequest(1, "quickJoin", new QuickJoinParams());

        Assert.DoesNotContain("useAvpf", text);
        Assert.DoesNotContain("srtpPolicy", text);
    }

    [Fact]
    public void RequestWithoutDataHasNoDataProperty()
    {
        string text = ProtooMessageCodec.EncodeRequest(1, "createTurnTransport", null);

        using JsonDocument document = JsonDocument.Parse(text);
        Assert.False(document.RootElement.TryGetProperty("data", out _));
    }

    [Fact]
    public void EncodedErrorResponseHasProtooShape()
    {
        string text = ProtooMessageCodec.EncodeErrorResponse(3, 410, "recv transport is unavailable");

        ProtooResponse response = Assert.IsType<ProtooResponse>(ProtooMessageCodec.Decode(text));
        Assert.Equal(3, response.Id);
        Assert.False(response.Ok);
        Assert.Equal(410, response.ErrorCode);
        Assert.Equal("recv transport is unavailable", response.ErrorReason);
    }

    [Fact]
    public void NonAsciiTextIsNotEscaped()
    {
        string text = ProtooMessageCodec.EncodeErrorResponse(1, 500, "你已被管理员移出当前语音频道。");

        Assert.Contains("你已被管理员移出当前语音频道。", text);
    }
}
