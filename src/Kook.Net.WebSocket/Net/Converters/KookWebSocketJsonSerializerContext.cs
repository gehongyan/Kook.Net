using System.Text.Json.Serialization;
using Kook.API.Gateway;

namespace Kook.Net.Converters;

/// <summary>
///     Provides JSON serialization context for Gateway models for Native AOT compatibility.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString)]
[JsonSerializable(typeof(ChannelBatchDeleteEventItem))]
[JsonSerializable(typeof(ChannelBatchUpdateEvent))]
[JsonSerializable(typeof(ChannelDeleteEvent))]
[JsonSerializable(typeof(ChannelSortCategory))]
[JsonSerializable(typeof(ChannelSortEvent))]
[JsonSerializable(typeof(DirectMessageButtonClickEvent))]
[JsonSerializable(typeof(DirectMessageDeleteEvent))]
[JsonSerializable(typeof(DirectMessageUpdateEvent))]
[JsonSerializable(typeof(EmbedsAppendEvent))]
[JsonSerializable(typeof(GatewayGroupMessageExtraData))]
[JsonSerializable(typeof(GatewayHelloPayload))]
[JsonSerializable(typeof(GatewayPersonMessageExtraData))]
[JsonSerializable(typeof(GatewayReconnectPayload))]
[JsonSerializable(typeof(GatewaySocketFrame))]
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
internal partial class KookWebSocketJsonSerializerContext : JsonSerializerContext
{
}
