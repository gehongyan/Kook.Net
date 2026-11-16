using System.Text.Json.Serialization;
using Kook.API.Gateway;

namespace Kook.Net.Contexts;

/// <summary>
///     Provides JSON serialization context for Gateway models for Native AOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString)]
// Gateway Models
[JsonSerializable(typeof(Kook.API.Gateway.ChannelBatchDeleteEventItem))]
[JsonSerializable(typeof(Kook.API.Gateway.ChannelBatchUpdateEvent))]
[JsonSerializable(typeof(Kook.API.Gateway.ChannelDeleteEvent))]
[JsonSerializable(typeof(Kook.API.Gateway.ChannelSortEvent))]
[JsonSerializable(typeof(DirectMessageButtonClickEvent))]
[JsonSerializable(typeof(DirectMessageDeleteEvent))]
[JsonSerializable(typeof(DirectMessageUpdateEvent))]
[JsonSerializable(typeof(EmbedsAppendEvent))]
[JsonSerializable(typeof(GatewayGroupMessageExtraData))]
[JsonSerializable(typeof(GatewayHelloPayload))]
[JsonSerializable(typeof(GatewayPersonMessageExtraData))]
[JsonSerializable(typeof(GatewayReconnectPayload))]
[JsonSerializable(typeof(GatewaySocketFrame))]
[JsonSerializable(typeof(GatewaySocketFrameType))]
[JsonSerializable(typeof(GatewaySystemEventExtraData))]
[JsonSerializable(typeof(GuildBanEvent))]
[JsonSerializable(typeof(GuildEmojiEvent))]
[JsonSerializable(typeof(GuildEvent))]
[JsonSerializable(typeof(GuildMemberAddEvent))]
[JsonSerializable(typeof(GuildMemberOnlineOfflineEvent))]
[JsonSerializable(typeof(GuildMemberRemoveEvent))]
[JsonSerializable(typeof(GuildMemberUpdateEvent))]
[JsonSerializable(typeof(GuildMuteDeafEvent))]
[JsonSerializable(typeof(GuildUpdateSelfEvent))]
[JsonSerializable(typeof(KMarkdownInfo))]
[JsonSerializable(typeof(LiveInfo))]
[JsonSerializable(typeof(LiveStatusChangeEvent))]
[JsonSerializable(typeof(LiveStreamUser))]
[JsonSerializable(typeof(MessageButtonClickEvent))]
[JsonSerializable(typeof(MessageDeleteEvent))]
[JsonSerializable(typeof(MessagePinEvent))]
[JsonSerializable(typeof(MessageUpdateEvent))]
[JsonSerializable(typeof(PrivateReaction))]
[JsonSerializable(typeof(Reaction))]
[JsonSerializable(typeof(SelfGuildEvent))]
[JsonSerializable(typeof(UserUpdateEvent))]
[JsonSerializable(typeof(UserVoiceEvent))]
// Gateway Events
[JsonSerializable(typeof(GatewayEvent<GatewayGroupMessageExtraData>))]
[JsonSerializable(typeof(GatewayEvent<GatewayPersonMessageExtraData>))]
[JsonSerializable(typeof(GatewayEvent<GatewaySystemEventExtraData>))]
[JsonSerializable(typeof(Kook.API.Gateway.ChannelSortCategory), TypeInfoPropertyName = "ProtocolChannelSortCategory")]
[JsonSerializable(typeof(Kook.API.Gateway.Reaction))]
[JsonSerializable(typeof(Kook.API.Channel))]
[JsonSerializable(typeof(Kook.API.Channel[]))]
[JsonSerializable(typeof(ChannelBatchDeleteEventItem[]))]
[JsonSerializable(typeof(Kook.API.Role))]
[JsonSerializable(typeof(System.Text.Json.JsonElement?))]
internal partial class KookWebSocketJsonSerializerContext : JsonSerializerContext;
