using System.Text;
using Kook.API.Rest;

namespace Kook.Audio.WebRtc;

/// <summary>
///     表示语音频道的信令网关令牌。
/// </summary>
/// <remarks>
///     <see cref="GatewayUrl"/> 中包含临时鉴权参数，请勿将其写入日志。
///     <see cref="ToString"/> 与 <see cref="RedactedGatewayUrl"/> 返回的是已脱敏的地址。
/// </remarks>
public sealed class VoiceGatewayToken
{
    internal VoiceGatewayToken(string gatewayUrl, int? pcVoiceSdk, int? iosVoiceSdk)
    {
        GatewayUrl = gatewayUrl;
        PcVoiceSdk = pcVoiceSdk;
        IosVoiceSdk = iosVoiceSdk;
    }

    /// <summary>
    ///     获取语音信令网关的 WebSocket 地址，其中包含临时鉴权参数。
    /// </summary>
    public string GatewayUrl { get; }

    /// <summary>
    ///     获取服务端返回的 PC 端语音 SDK 标记。
    /// </summary>
    /// <remarks>
    ///     该值的含义尚未确认，推测与语音 SDK 的灰度版本有关。
    /// </remarks>
    public int? PcVoiceSdk { get; }

    /// <summary>
    ///     获取服务端返回的 iOS 端语音 SDK 标记。
    /// </summary>
    /// <remarks>
    ///     该值的含义尚未确认，推测与语音 SDK 的灰度版本有关。
    /// </remarks>
    public int? IosVoiceSdk { get; }

    /// <summary>
    ///     获取已脱敏的网关地址，仅保留协议、主机、路径以及查询参数的名称。
    /// </summary>
    public string RedactedGatewayUrl => Redact(GatewayUrl);

    internal static VoiceGatewayToken Create(GetVoiceGatewayResponse model) =>
        new(model.GatewayUrl, model.PcVoiceSdk, model.IosVoiceSdk);

    internal static string Redact(string gatewayUrl)
    {
        if (!Uri.TryCreate(gatewayUrl, UriKind.Absolute, out Uri? uri))
            return "<invalid gateway url>";

        StringBuilder builder = new();
        builder.Append(uri.Scheme).Append("://").Append(uri.Authority).Append(uri.AbsolutePath);
        if (uri.Query.Length > 1)
        {
            IEnumerable<string> names = uri.Query.Substring(1)
                .Split(['&'], StringSplitOptions.RemoveEmptyEntries)
                .Select(pair => pair.Split(['='], 2)[0]);
            builder.Append("?<").Append(string.Join(",", names)).Append("=redacted>");
        }
        return builder.ToString();
    }

    /// <inheritdoc />
    public override string ToString() => $"VoiceGatewayToken {{ GatewayUrl = {RedactedGatewayUrl} }}";
}
