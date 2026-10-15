using System;
using System.Collections.Generic;
using Kook.Audio.WebRtc.Media;
using Kook.Audio.WebRtc.Signaling;
using Xunit;

namespace Kook.Audio.WebRtc;

/// <summary>
///     Tests for <see cref="WebRtcSdpBuilder"/>.
/// </summary>
[Trait("Category", "Unit")]
public class WebRtcSdpBuilderTests
{
    private static VoiceTransportParameters CreateTransport(string algorithm = "sha-256") => new()
    {
        Id = "transport-1",
        IceParameters = new VoiceIceParameters { UsernameFragment = "ufrag", Password = "pwd", IceLite = true },
        IceCandidates =
        [
            new VoiceIceCandidate
            {
                Foundation = "udpcandidate", Ip = "10.0.0.1", Port = 40078, Priority = 1076225279, Protocol = "udp", Type = "host"
            },
            new VoiceIceCandidate
            {
                Foundation = "udpcandidate", Ip = "203.0.113.5", Port = 40079, Priority = 1076199679, Protocol = "UDP", Type = "host"
            }
        ],
        DtlsParameters = new VoiceDtlsParameters
        {
            Role = "auto",
            Fingerprints =
            [
                new VoiceDtlsFingerprint { Algorithm = "sha-1", Value = "AA:AA" },
                new VoiceDtlsFingerprint { Algorithm = algorithm, Value = "BB:BB" }
            ]
        }
    };

    private static string[] Lines(string sdp) => sdp.Split(["\r\n"], StringSplitOptions.RemoveEmptyEntries);

    [Fact]
    public void AnswerDeclaresAnIceLiteServerThatIsPassiveForDtls()
    {
        string sdp = WebRtcSdpBuilder.BuildAnswer("0", "recvonly", 111, "minptime=10;useinbandfec=1", CreateTransport());

        List<string> lines = [.. Lines(sdp)];
        Assert.Contains("a=ice-lite", lines);
        Assert.Contains("a=setup:passive", lines);
        Assert.Contains("a=rtcp-mux", lines);
        Assert.Contains("a=ice-ufrag:ufrag", lines);
        Assert.Contains("a=ice-pwd:pwd", lines);
    }

    [Theory]
    [InlineData("recvonly")]
    [InlineData("sendonly")]
    public void AnswerCarriesTheRequestedDirectionMidAndCodec(string direction)
    {
        string sdp = WebRtcSdpBuilder.BuildAnswer("3", direction, 100, "minptime=10", CreateTransport());

        List<string> lines = [.. Lines(sdp)];
        Assert.Contains($"a={direction}", lines);
        Assert.Contains("a=mid:3", lines);
        Assert.Contains("a=group:BUNDLE 3", lines);
        Assert.Contains("m=audio 7 UDP/TLS/RTP/SAVPF 100", lines);
        Assert.Contains("a=rtpmap:100 opus/48000/2", lines);
        Assert.Contains("a=fmtp:100 minptime=10", lines);
    }

    [Fact]
    public void AnswerPrefersTheSha256Fingerprint()
    {
        string sdp = WebRtcSdpBuilder.BuildAnswer("0", "recvonly", 111, "x", CreateTransport());

        List<string> lines = [.. Lines(sdp)];
        Assert.Contains("a=fingerprint:sha-256 BB:BB", lines);
        Assert.DoesNotContain("a=fingerprint:sha-1 AA:AA", lines);
    }

    [Fact]
    public void AnswerFallsBackToTheFirstFingerprintWithoutSha256()
    {
        VoiceTransportParameters transport = CreateTransport("sha-512");

        string sdp = WebRtcSdpBuilder.BuildAnswer("0", "recvonly", 111, "x", transport);

        Assert.Contains("a=fingerprint:sha-1 AA:AA", Lines(sdp));
    }

    [Fact]
    public void AnswerListsEveryCandidateWithLowercaseProtocol()
    {
        string sdp = WebRtcSdpBuilder.BuildAnswer("0", "recvonly", 111, "x", CreateTransport());

        List<string> lines = [.. Lines(sdp)];
        Assert.Contains("a=candidate:udpcandidate 1 udp 1076225279 10.0.0.1 40078 typ host", lines);
        Assert.Contains("a=candidate:udpcandidate 1 udp 1076199679 203.0.113.5 40079 typ host", lines);
        Assert.Contains("a=end-of-candidates", lines);
    }

    [Fact]
    public void AnswerUsesCrlfLineEndings()
    {
        string sdp = WebRtcSdpBuilder.BuildAnswer("0", "recvonly", 111, "x", CreateTransport());

        Assert.EndsWith("\r\n", sdp);
        Assert.DoesNotContain("\n\n", sdp);
        Assert.Equal(0, CountBareLineFeeds(sdp));
    }

    [Fact]
    public void AnswerRequiresAtLeastOneFingerprint()
    {
        VoiceTransportParameters transport = CreateTransport();
        transport.DtlsParameters.Fingerprints = [];

        Assert.Throws<InvalidOperationException>(() =>
            WebRtcSdpBuilder.BuildAnswer("0", "recvonly", 111, "x", transport));
    }

    private static int CountBareLineFeeds(string text)
    {
        int count = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n' && (i == 0 || text[i - 1] != '\r'))
                count++;
        }
        return count;
    }
}
