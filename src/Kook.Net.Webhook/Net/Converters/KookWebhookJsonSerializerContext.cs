using System.Text.Json.Serialization;
using Kook.API.Webhook;

namespace Kook.Net.Contexts;

/// <summary>
///     Provides JSON serialization context for Webhook models for Native AOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString)]
[JsonSerializable(typeof(GatewayChallengeFrame))]
[JsonSerializable(typeof(GatewayEncryptedFrame))]
internal partial class KookWebhookJsonSerializerContext : JsonSerializerContext;
