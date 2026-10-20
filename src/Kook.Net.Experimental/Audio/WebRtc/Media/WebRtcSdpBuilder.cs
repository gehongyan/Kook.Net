using System.Text;
using Kook.Audio.WebRtc.Signaling;

namespace Kook.Audio.WebRtc.Media;

/// <summary>
///     用 <c>quickJoin</c> 返回的传输参数拼出远端 SDP answer，供 <c>RTCPeerConnection</c> 使用。
///     服务端是 ICE-lite，DTLS 角色为 passive（客户端为 active）。
/// </summary>
internal static class WebRtcSdpBuilder
{
    public static string BuildAnswer(string mid, string direction, int payloadType, string fmtp,
        VoiceTransportParameters transport)
    {
        VoiceDtlsFingerprint fingerprint =
            transport.DtlsParameters.Fingerprints.FirstOrDefault(f =>
                string.Equals(f.Algorithm, "sha-256", StringComparison.OrdinalIgnoreCase))
            ?? transport.DtlsParameters.Fingerprints.FirstOrDefault()
            ?? throw new InvalidOperationException("The transport has no DTLS fingerprints.");

        StringBuilder sdp = new();

        void Line(string text) => sdp.Append(text).Append("\r\n");

        Line("v=0");
        Line("o=- 10000 1 IN IP4 127.0.0.1");
        Line("s=-");
        Line("t=0 0");
        Line("a=ice-lite");
        Line($"a=group:BUNDLE {mid}");
        Line("a=msid-semantic: WMS *");
        Line($"m=audio 7 UDP/TLS/RTP/SAVPF {payloadType}");
        Line("c=IN IP4 127.0.0.1");
        Line("a=rtcp:9 IN IP4 0.0.0.0");
        Line($"a=ice-ufrag:{transport.IceParameters.UsernameFragment}");
        Line($"a=ice-pwd:{transport.IceParameters.Password}");
        Line($"a=fingerprint:{fingerprint.Algorithm.ToLowerInvariant()} {fingerprint.Value}");
        Line("a=setup:passive");
        Line($"a=mid:{mid}");
        Line($"a={direction}");
        Line("a=rtcp-mux");
        Line($"a=rtpmap:{payloadType} opus/48000/2");
        Line($"a=fmtp:{payloadType} {fmtp}");
        Line($"a=rtcp-fb:{payloadType} nack");
        foreach (VoiceIceCandidate candidate in transport.IceCandidates)
        {
            Line($"a=candidate:{candidate.Foundation} 1 {candidate.Protocol.ToLowerInvariant()} {candidate.Priority} "
                + $"{candidate.Ip} {candidate.Port} typ {candidate.Type}");
        }
        Line("a=end-of-candidates");
        return sdp.ToString();
    }
}
