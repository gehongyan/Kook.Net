using Kook.Net.Converters;
using System.Text.Json.Serialization;

namespace Kook.API.Rest;

internal class DeleteUserChatParams
{
    [JsonPropertyName("chat_code")]
    [JsonConverter(typeof(ChatCodeConverter))]
    public required Guid ChatCode { get; set; }

    public static implicit operator DeleteUserChatParams(Guid chatCode) => new() { ChatCode = chatCode };
}
