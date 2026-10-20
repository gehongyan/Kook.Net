using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="VoiceGatewayToken"/>.
/// </summary>
[Trait("Category", "Unit")]
public class VoiceGatewayTokenTests
{
    private const string Url = "wss://gw.example.invalid/gateway/v1?token=SECRET-TOKEN&roomId=123&peerId=456&sign=SECRET-SIGN";

    [Fact]
    public void RedactedUrlKeepsHostPathAndQueryNamesOnly()
    {
        VoiceGatewayToken token = new(Url, 2, 2);

        Assert.Equal("wss://gw.example.invalid/gateway/v1?<token,roomId,peerId,sign=redacted>", token.RedactedGatewayUrl);
        Assert.DoesNotContain("SECRET", token.RedactedGatewayUrl);
    }

    [Fact]
    public void ToStringDoesNotLeakCredentials()
    {
        VoiceGatewayToken token = new(Url, 2, 2);

        string text = token.ToString();

        Assert.DoesNotContain("SECRET", text);
        Assert.DoesNotContain("123", text);
        Assert.Contains("gw.example.invalid", text);
    }

    [Fact]
    public void GatewayUrlIsKeptIntact()
    {
        VoiceGatewayToken token = new(Url, null, null);

        Assert.Equal(Url, token.GatewayUrl);
        Assert.Null(token.PcVoiceSdk);
        Assert.Null(token.IosVoiceSdk);
    }

    [Fact]
    public void UrlWithoutQueryIsNotChanged() =>
        Assert.Equal("wss://gw.example.invalid/gateway/v1",
            VoiceGatewayToken.Redact("wss://gw.example.invalid/gateway/v1"));

    [Fact]
    public void InvalidUrlIsReportedWithoutEchoingIt() =>
        Assert.Equal("<invalid gateway url>", VoiceGatewayToken.Redact("not a url SECRET"));
}
