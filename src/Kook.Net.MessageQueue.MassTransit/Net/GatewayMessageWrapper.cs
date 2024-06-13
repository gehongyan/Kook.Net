using System.Text.Json;

namespace Kook.Net.Queue.MassTransit;

/// <summary>
///
/// </summary>
public record GatewayMessageWrapper
{
    /// <summary>
    ///
    /// </summary>
    public JsonElement Payload { get; set; }
    /// <summary>
    ///
    /// </summary>
    public int Sequence { get; set; }
}
